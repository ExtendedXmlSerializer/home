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
	sealed class ImmutableHashSetExtension : ISerializerExtension
	{
		public static ImmutableHashSetExtension Default { get; } = new ImmutableHashSetExtension();

		ImmutableHashSetExtension() : this(new IsAssignableGenericSpecification(typeof(ImmutableHashSet<>))) {}

		readonly ISpecification<TypeInfo> _specification;

		public ImmutableHashSetExtension(ISpecification<TypeInfo> specification) => _specification = specification;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContentsWith<ImmutableHashSets>()
			            .When(_specification)
			            .Decorate<IGenericTypes, GenericTypes>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class GenericTypes : IGenericTypes
		{
			readonly static TypeInfo Check = typeof(ImmutableHashSet).GetTypeInfo();
			readonly static ImmutableArray<TypeInfo> Type = typeof(ImmutableHashSet<>).GetTypeInfo()
			                                                                          .Yield()
			                                                                          .ToImmutableArray();

			readonly IGenericTypes _types;

			public GenericTypes(IGenericTypes types) => _types = types;

			public ImmutableArray<TypeInfo> Get(IIdentity parameter)
			{
				var type   = _types.Get(parameter);
				var result = Equals(type.Only(), Check) ? Type : type;
				return result;
			}
		}

		sealed class ImmutableHashSets : Collections
		{
			public ImmutableHashSets(RuntimeSerializers serializers, Contents contents) : base(serializers, contents) {}
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

				public object Get(IFormatReader parameter) => _reader.Get(parameter).ToImmutableHashSet();
			}
		}
	}
}
