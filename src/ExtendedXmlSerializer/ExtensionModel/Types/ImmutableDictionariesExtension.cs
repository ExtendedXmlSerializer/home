using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ImmutableDictionariesExtension : ISerializerExtension
	{
		public static ImmutableDictionariesExtension Default { get; } = new ImmutableDictionariesExtension();

		ImmutableDictionariesExtension() : this(new IsAssignableGenericSpecification(typeof(ImmutableDictionary<,>))) {}

		readonly ISpecification<TypeInfo> _specification;

		public ImmutableDictionariesExtension(ISpecification<TypeInfo> specification) => _specification = specification;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContentsWith<Contents>()
			            .When(_specification)
			            .Decorate<IGenericTypes, GenericTypes>()
			            .Decorate<IConstructorLocator>(Register);

		IConstructorLocator Register(IServiceProvider arg1, IConstructorLocator previous)
			=> new ConstructorLocator(_specification, previous);

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class ConstructorLocator : DictionaryConstructorLocator, IConstructorLocator
		{
			public ConstructorLocator(ISpecification<TypeInfo> specification, IConstructorLocator previous)
				: base(specification, previous, typeof(Adapter<,>)) {}
		}

		sealed class Adapter<TKey, TValue> : Dictionary<TKey, TValue>, IActivationAware
		{
			public object Get() => this.ToImmutableDictionary();
		}

		sealed class GenericTypes : IGenericTypes
		{
			readonly static TypeInfo Check = typeof(ImmutableDictionary).GetTypeInfo();
			readonly static ImmutableArray<TypeInfo> Type = typeof(ImmutableDictionary<,>).GetTypeInfo()
			                                                                              .Yield()
			                                                                              .ToImmutableArray();

			readonly IGenericTypes _types;

			public GenericTypes(IGenericTypes types) => _types = types;

			public ImmutableArray<TypeInfo> Get(IIdentity parameter)
			{
				var type   = _types.Get(parameter);
				var result = type.Only()?.Equals(Check) ?? false ? Type : type;
				return result;
			}
		}

		sealed class Contents : IContents
		{
			readonly IContents                  _contents;
			readonly IGeneric<IReader, IReader> _readers;
			readonly Type                       _type;

			[UsedImplicitly]
			public Contents(IContents contents) : this(contents, Readers.Instance, typeof(Dictionary<,>)) {}

			public Contents(IContents contents, IGeneric<IReader, IReader> readers, Type type)
			{
				_contents = contents;
				_readers  = readers;
				_type     = type;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var types      = DictionaryPairTypesLocator.Default.Get(parameter).GetValueOrDefault();
				var type       = _type.MakeGenericType(types.KeyType, types.ValueType);
				var serializer = _contents.Get(type);
				var result     = new Serializer(_readers.Get(types.KeyType, types.ValueType)(serializer), serializer);
				return result;
			}

			sealed class Readers : Generic<IReader, IReader>
			{
				public static Readers Instance { get; } = new Readers();

				Readers() : base(typeof(Reader<,>)) {}
			}

			sealed class Reader<TKey, TValue> : IReader
			{
				readonly IReader _reader;

				[UsedImplicitly]
				public Reader(IReader reader) => _reader = reader;

				public object Get(IFormatReader parameter)
					=> _reader.Get(parameter).AsValid<IDictionary<TKey, TValue>>().ToImmutableDictionary();
			}
		}
	}
}