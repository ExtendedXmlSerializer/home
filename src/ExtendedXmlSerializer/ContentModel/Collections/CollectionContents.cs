using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class CollectionContents : ICollectionContents
	{
		readonly IInstanceMemberSerializations _instances;
		readonly IEnumerators                  _enumerators;
		readonly IInnerContentServices         _contents;

		public CollectionContents(IInstanceMemberSerializations instances, IEnumerators enumerators,
		                          IInnerContentServices contents)
		{
			_instances   = instances;
			_enumerators = enumerators;
			_contents    = contents;
		}

		public ISerializer Get(CollectionContentInput parameter)
		{
			var serialization = _instances.Get(parameter.Classification);
			var handler = new CollectionWithMembersInnerContentHandler(new
				                                                           MemberInnerContentHandler(serialization,
				                                                                                     _contents,
				                                                                                     _contents),
			                                                           new CollectionInnerContentHandler(parameter.Item,
			                                                                                             _contents));
			var reader = _contents.Create(parameter.Classification, handler);
			var writer = new MemberedCollectionWriter(new MemberListWriter(serialization),
			                                          new EnumerableWriter(_enumerators, parameter.Item).Adapt());
			var result = new Serializer(reader, writer);
			return result;
		}
	}
}