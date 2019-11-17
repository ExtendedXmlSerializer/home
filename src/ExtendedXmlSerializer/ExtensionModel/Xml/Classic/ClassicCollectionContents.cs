using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicCollectionContents : ICollectionContents
	{
		readonly IInnerContentServices _contents;
		readonly IEnumerators          _enumerators;

		public ClassicCollectionContents(IInnerContentServices contents, IEnumerators enumerators)
		{
			_contents    = contents;
			_enumerators = enumerators;
		}

		public ContentModel.ISerializer Get(CollectionContentInput parameter)
			=> new ContentModel.Serializer(_contents.Create(parameter.Classification,
			                                   new CollectionInnerContentHandler(parameter.Item, _contents)),
			                  new EnumerableWriter(_enumerators, parameter.Item).Adapt());
	}
}