using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberContents : DecoratedSource<IMember, ISerializer>, IMemberContents
	{
		readonly static ISpecification<IMember> Specification = VariableTypeMemberSpecifications.Default.IfAssigned();

		public MemberContents(DefaultMemberContents @default, VariableTypeMemberContents variable,
		                      RegisteredMemberContents registered)
			: base(@default.Let(Specification, variable)
			               .Let(registered, registered)) {}
	}
}