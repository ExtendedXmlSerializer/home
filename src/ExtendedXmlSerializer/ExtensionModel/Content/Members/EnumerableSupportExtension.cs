using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ISerializers = ExtendedXmlSerializer.ContentModel.Content.ISerializers;
using RuntimeSerializer = ExtendedXmlSerializer.ContentModel.RuntimeSerializer;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class EnumerableSupportExtension : ISerializerExtension
	{
		public static EnumerableSupportExtension Default { get; } = new EnumerableSupportExtension();

		EnumerableSupportExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterWithDependencies<MemberCore>()
			            .Decorate<IMemberContentsCore, MemberCore>()
			            .RegisterWithDependencies<EnumerableAwareSerializers>()
			            .Decorate<ISerializers, EnumerableAwareSerializers>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class MemberCore : DecoratedSource<IMember, ISerializer>, IMemberContentsCore
		{
			public MemberCore(IMemberContentsCore core, ExplicitEnumerableMemberContents enumerable)
				: base(core.Let(ExplicitEnumerableMemberSpecification.Instance, enumerable)) {}
		}

		sealed class ExplicitEnumerableSpecification : ISpecification<TypeInfo>
		{
			public static ExplicitEnumerableSpecification Instance { get; } = new ExplicitEnumerableSpecification();

			ExplicitEnumerableSpecification() : this(CollectionItemTypeLocator.Default) {}

			readonly ICollectionItemTypeLocator _locator;
			readonly Type                       _definition;

			public ExplicitEnumerableSpecification(ICollectionItemTypeLocator locator)
				: this(locator, typeof(IEnumerable<>)) {}

			public ExplicitEnumerableSpecification(ICollectionItemTypeLocator locator, Type definition)
			{
				_locator    = locator;
				_definition = definition;
			}

			public bool IsSatisfiedBy(TypeInfo parameter)
			{
				var itemType = _locator.Get(parameter);
				var result   = itemType != null && _definition.MakeGenericType(itemType) == parameter;
				return result;
			}
		}

		sealed class ExplicitEnumerableMemberSpecification : DecoratedSpecification<IMember>
		{
			public static ExplicitEnumerableMemberSpecification Instance { get; } =
				new ExplicitEnumerableMemberSpecification();

			ExplicitEnumerableMemberSpecification()
				: base(new MemberTypeSpecification(ExplicitEnumerableSpecification.Instance)) {}
		}

		sealed class ExplicitEnumerableMemberContents : IMemberContents
		{
			readonly IProperty<TypeInfo> _type;
			readonly ISerializer         _runtime;
			readonly IContents           _contents;

			[UsedImplicitly]
			public ExplicitEnumerableMemberContents(ISerializer runtime, IContents contents)
				: this(ExplicitTypeProperty.Default, runtime, contents) {}

			public ExplicitEnumerableMemberContents(IProperty<TypeInfo> type, ISerializer runtime, IContents contents)
			{
				_type     = type;
				_runtime  = runtime;
				_contents = contents;
			}

			public ISerializer Get(IMember parameter)
			{
				var implementation = EnumerableImplementation.Instance.Get(parameter.MemberType);
				var element        = CollectionItemTypeLocator.Default.Get(implementation);
				var actual         = typeof(List<>).MakeGenericType(element);
				var contents       = _contents.Get(actual);

				var specification =
					ContentModel.Reflection.VariableTypeSpecification.Defaults.Get(parameter.MemberType);
				var variable = new VariableTypedMemberWriter(specification, _runtime, contents, _type);
				var writer   = Writers.Instance.Get(element)(variable);
				var result   = new Serializer(contents, writer);
				return result;
			}
		}

		sealed class Lists<T> : ReferenceCache<IEnumerable, List<T>>
		{
			public static Lists<T> Instance { get; } = new Lists<T>();

			Lists() : base(key => key as List<T> ?? key.Cast<T>().ToList()) {}
		}

		sealed class EnumerableImplementation : IAlteration<TypeInfo>
		{
			public static EnumerableImplementation Instance { get; } = new EnumerableImplementation();

			EnumerableImplementation() : this(AllInterfaces.Default, typeof(IEnumerable<>)) {}

			readonly IAllInterfaces _all;
			readonly Type           _definition;

			public EnumerableImplementation(IAllInterfaces all, Type definition)
			{
				_all        = all;
				_definition = definition;
			}

			public TypeInfo Get(TypeInfo parameter)
			{
				foreach (var definition in _all.Get(parameter)
				                               .Where(x => x.IsGenericType))
				{
					if (definition.GetGenericTypeDefinition() == _definition)
					{
						return definition;
					}
				}

				return null;
			}
		}

		sealed class Writers : Generic<IWriter, IWriter>
		{
			public static Writers Instance { get; } = new Writers();

			Writers() : base(typeof(Writer<>)) {}
		}

		sealed class Writer<T> : IWriter
		{
			readonly IWriter                                       _writer;
			readonly IParameterizedSource<IEnumerable<T>, List<T>> _lists;

			[UsedImplicitly]
			public Writer(IWriter writer) : this(writer, Lists<T>.Instance) {}

			public Writer(IWriter writer, IParameterizedSource<IEnumerable<T>, List<T>> lists)
			{
				_writer = writer;
				_lists  = lists;
			}

			public void Write(IFormatWriter writer, object instance)
			{
				_writer.Write(writer, _lists.Get(instance.AsValid<IEnumerable<T>>()));
			}
		}

		sealed class EnumerableElementType : IAlteration<TypeInfo>
		{
			public static EnumerableElementType Instance { get; } = new EnumerableElementType();

			EnumerableElementType() : this(EnumerableImplementation.Instance, CollectionItemTypeLocator.Default) {}

			readonly IParameterizedSource<TypeInfo, TypeInfo> _implementation;
			readonly IParameterizedSource<TypeInfo, TypeInfo> _locator;

			public EnumerableElementType(IParameterizedSource<TypeInfo, TypeInfo> implementation,
			                             IParameterizedSource<TypeInfo, TypeInfo> locator)
			{
				_implementation = implementation;
				_locator        = locator;
			}

			public TypeInfo Get(TypeInfo parameter) => _locator.Get(_implementation.Get(parameter));
		}

		sealed class IsEnumerableElement : AllSpecification<TypeInfo>
		{
			public IsEnumerableElement(IConverters converters)
				: base(converters.IfAssigned().Inverse(), EnumerableImplementation.Instance.IfAssigned()) {}
		}

		sealed class EnumerableAwareSerializers : ISerializers
		{
			readonly ISpecification<TypeInfo> _specification;
			readonly IContents                _contents;
			readonly IAlteration<TypeInfo>    _element;
			readonly ISerializers             _previous;

			[UsedImplicitly]
			public EnumerableAwareSerializers(IsEnumerableElement specification, IContents contents,
			                                  ISerializers previous)
				: this(specification, contents, EnumerableElementType.Instance, previous) {}

			// ReSharper disable once TooManyDependencies
			public EnumerableAwareSerializers(IsEnumerableElement specification, IContents contents,
			                                  IAlteration<TypeInfo> element, ISerializers previous)
			{
				_specification = specification;
				_contents      = contents;
				_element       = element;
				_previous      = previous;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var contents = _contents.Get(parameter);
				if (contents is RuntimeSerializer && _specification.IsSatisfiedBy(parameter))
				{
					var element = _element.Get(parameter);

					var actual   = typeof(List<>).MakeGenericType(element);
					var previous = _previous.Get(actual);
					var result   = new Serializer(previous, Writers.Instance.Get(element)(previous));
					return result;
				}

				{
					var result = _previous.Get(parameter);
					return result;
				}
			}
		}
	}
}