using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberHandler : IMemberHandler
	{
		readonly IMemberAssignment _assignment;

		public MemberHandler(IMemberAssignment assignment) => _assignment = assignment;

		public void Handle(IInnerContent contents, IMemberSerializer member)
		{
			_assignment.Assign(contents, member.Access, member.Get(contents.Get()));
		}
	}
}