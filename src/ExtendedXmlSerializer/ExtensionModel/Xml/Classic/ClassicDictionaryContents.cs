using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicDictionaryContents : IContents
	{
		readonly IInnerContentServices  _contents;
		readonly IDictionaryEnumerators _enumerators;
		readonly IDictionaryEntries     _entries;

		public ClassicDictionaryContents(IInnerContentServices contents, IDictionaryEnumerators enumerators,
		                                 IDictionaryEntries entries)
		{
			_contents    = contents;
			_enumerators = enumerators;
			_entries     = entries;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var entry  = _entries.Get(parameter);
			var reader = _contents.Create(parameter, new CollectionInnerContentHandler(entry, _contents));
			var result = new Serializer(reader, new EnumerableWriter(_enumerators, entry).Adapt());
			return result;
		}
	}
}