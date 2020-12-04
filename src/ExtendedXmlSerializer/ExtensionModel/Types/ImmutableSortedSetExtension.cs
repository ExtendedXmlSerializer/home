using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ImmutableSortedSetExtension : ISerializerExtension
	{
		public static ImmutableSortedSetExtension Default { get; } = new ImmutableSortedSetExtension();

		ImmutableSortedSetExtension() : this(new IsAssignableGenericSpecification(typeof(ImmutableSortedSet<>))) {}

		readonly ISpecification<TypeInfo> _specification;

		public ImmutableSortedSetExtension(ISpecification<TypeInfo> specification) => _specification = specification;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContentsWith<ImmutableSortedSets>()
			            .When(_specification)
			            .Decorate<IGenericTypes, GenericTypes>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class GenericTypes : IGenericTypes
		{
			readonly static TypeInfo Check = typeof(ImmutableSortedSet).GetTypeInfo();
			readonly static ImmutableArray<TypeInfo> Type = typeof(ImmutableSortedSet<>).GetTypeInfo()
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

		sealed class ImmutableSortedSets : Collections
		{
			public ImmutableSortedSets(RuntimeSerializers serializers, Contents contents)
				: base(serializers, contents) {}
		}

		sealed class Contents : ICollectionContents
		{
			readonly IInnerContentServices _contents;
			readonly IEnumerators          _enumerators;

			public Contents(IInnerContentServices contents, IEnumerators enumerators)
			{
				_contents    = contents;
				_enumerators = enumerators;
			}

			public ISerializer Get(CollectionContentInput parameter)
				=> new Serializer(Readers.Instance.Get(parameter.ItemType)(_contents, parameter.Item),
				                  new EnumerableWriter(_enumerators, parameter.Item).Adapt());

			sealed class Readers : Generic<IInnerContentServices, IReader, IReader>
			{
				public static Readers Instance { get; } = new Readers();

				Readers() : base(typeof(Reader<>)) {}
			}

			sealed class Reader<T> : IReader
			{
				readonly IReader<Collection<T>> _reader;

				[UsedImplicitly]
				public Reader(IInnerContentServices services, IReader item)
					: this(services.CreateContents<Collection<T>>(new CollectionInnerContentHandler(item, services))) {}

				Reader(IReader<Collection<T>> reader) => _reader = reader;

				public object Get(IFormatReader parameter) => _reader.Get(parameter).ToImmutableSortedSet();
			}
		}
	}

}
