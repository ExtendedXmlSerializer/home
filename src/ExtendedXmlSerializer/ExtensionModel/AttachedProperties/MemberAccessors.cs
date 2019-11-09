using ExtendedXmlSerializer.ContentModel.Members;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	sealed class MemberAccessors : IMemberAccessors
	{
		readonly IAllowedMemberValues _allow;
		readonly IMemberAccessors     _accessors;

		public MemberAccessors(IAllowedMemberValues allow, IMemberAccessors accessors)
		{
			_allow     = allow;
			_accessors = accessors;
		}

		public IMemberAccess Get(IMember parameter) => parameter is AttachedMember member
			                                               ? new MemberAccess(_allow.Get(member.Metadata), member)
			                                               : _accessors.Get(parameter);
	}
}