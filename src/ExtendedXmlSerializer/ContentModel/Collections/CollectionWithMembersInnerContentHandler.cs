using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class CollectionWithMembersInnerContentHandler : IInnerContentHandler
	{
		readonly MemberInnerContentHandler     _members;
		readonly CollectionInnerContentHandler _collection;

		public CollectionWithMembersInnerContentHandler(MemberInnerContentHandler members,
		                                                CollectionInnerContentHandler collection)
		{
			_members    = members;
			_collection = collection;
		}

		public bool IsSatisfiedBy(IInnerContent parameter)
			=> _members.IsSatisfiedBy(parameter) || _collection.IsSatisfiedBy(parameter);
	}
}