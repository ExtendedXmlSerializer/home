using System.Collections.Immutable;
using System.Collections.ObjectModel;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ImmutableArrayContents : ICollectionContents
	{
		readonly IInnerContentServices _contents;
		readonly IEnumerators          _enumerators;

		public ImmutableArrayContents(IInnerContentServices contents, IEnumerators enumerators)
		{
			_contents    = contents;
			_enumerators = enumerators;
		}

		public ISerializer Get(CollectionContentInput parameter)
			=> new Serializer(Readers.Default.Get(parameter.ItemType)
			                         .Invoke(_contents, parameter.Item),
			                  new EnumerableWriter(_enumerators,
			                                       parameter.Item)
				                  .Adapt());

		sealed class Readers : Generic<IInnerContentServices, IReader, IReader>
		{
			public static Readers Default { get; } = new Readers();

			Readers() : base(typeof(Reader<>)) {}
		}

		sealed class Reader<T> : IReader
		{
			readonly IReader<Collection<T>> _reader;

			[UsedImplicitly]
			public Reader(IInnerContentServices services, IReader item)
				: this(services.CreateContents<Collection<T>>(new CollectionInnerContentHandler(item, services))) {}

			Reader(IReader<Collection<T>> reader) => _reader = reader;

			public object Get(IFormatReader parameter) => _reader.Get(parameter)
			                                                     .ToImmutableArray();
		}
	}
}