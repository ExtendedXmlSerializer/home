using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class DefaultMemberContents : IMemberContents
	{
		readonly IContents _contents;

		public DefaultMemberContents(IContents contents) => _contents = contents;

		public ISerializer Get(IMember parameter) => _contents.Get(parameter.MemberType);
	}
}