using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class DictionaryContents : IContents
	{
		readonly IInstanceMemberSerializations _instances;
		readonly IDictionaryEnumerators        _enumerators;
		readonly IDictionaryEntries            _entries;
		readonly IInnerContentServices         _contents;

		// ReSharper disable once TooManyDependencies
		public DictionaryContents(IInstanceMemberSerializations instances, IDictionaryEnumerators enumerators,
		                          IDictionaryEntries entries, IInnerContentServices contents)
		{
			_instances   = instances;
			_enumerators = enumerators;
			_entries     = entries;
			_contents    = contents;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var members = _instances.Get(parameter);
			var entry   = _entries.Get(parameter);

			var handler =
				new CollectionWithMembersInnerContentHandler(new MemberInnerContentHandler(_instances.Get(parameter),
				                                                                           _contents, _contents),
				                                             new CollectionInnerContentHandler(entry, _contents));
			var reader = _contents.Create(parameter, handler);
			var writer =
				new MemberedCollectionWriter(new MemberListWriter(members),
				                             new EnumerableWriter(_enumerators, entry).Adapt());
			var result = new Serializer(reader, writer);
			return result;
		}
	}
}