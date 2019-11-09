using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberAssignment : IMemberAssignment
	{
		public static MemberAssignment Default { get; } = new MemberAssignment();

		MemberAssignment() {}

		public void Assign(IInnerContent contents, IMemberAccess access, object value)
			=> access.Assign(contents.Current, value);
	}
}