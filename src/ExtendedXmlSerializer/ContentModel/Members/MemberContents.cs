using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface IMemberContentsCore : IMemberContents {}

	[UsedImplicitly]
	sealed class MemberContentsCore : DecoratedSource<IMember, ISerializer>, IMemberContentsCore
	{
		readonly static ISpecification<IMember> Specification = VariableTypeMemberSpecifications.Default.IfAssigned();

		public MemberContentsCore(DefaultMemberContents @default, VariableTypeMemberContents variable)
			: base(@default.Let(Specification, variable)) {}
	}

	[UsedImplicitly]
	sealed class MemberContents : DecoratedSource<IMember, ISerializer>, IMemberContents
	{
		public MemberContents(IMemberContentsCore core, RegisteredMemberContents registered)
			: base(core.Let(registered, registered)) {}
	}
}