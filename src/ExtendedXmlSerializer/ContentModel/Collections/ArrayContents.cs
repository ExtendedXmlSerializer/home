using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class ArrayContents : ICollectionContents
	{
		readonly IInnerContentServices _contents;
		readonly IEnumerators          _enumerators;
		readonly IClassification       _classification;

		public ArrayContents(IInnerContentServices contents, IEnumerators enumerators, IClassification classification)
		{
			_contents       = contents;
			_enumerators    = enumerators;
			_classification = classification;
		}

		public ISerializer Get(CollectionContentInput parameter)
			=> new Serializer(new ArrayReader(_contents, _classification, parameter.Classification, parameter.Item),
			                  new EnumerableWriter(_enumerators, parameter.Item).Adapt());
	}

	// ATTRIBUTION: https://stackoverflow.com/a/1183019/3602057
}