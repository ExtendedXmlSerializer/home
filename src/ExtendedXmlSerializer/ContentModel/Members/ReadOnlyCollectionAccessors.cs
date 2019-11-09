using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class ReadOnlyCollectionAccessors : IParameterizedSource<IMember, IMemberAccess>
	{
		readonly IAllowedMemberValues _members;
		readonly IGetterFactory       _getter;
		readonly IAddDelegates        _add;

		[UsedImplicitly]
		public ReadOnlyCollectionAccessors(IAllowedMemberValues members)
			: this(members, GetterFactory.Default, AddDelegates.Default) {}

		public ReadOnlyCollectionAccessors(IAllowedMemberValues members, IGetterFactory getter, IAddDelegates add)
		{
			_members = members;
			_getter  = getter;
			_add     = add;
		}

		public IMemberAccess Get(IMember parameter)
			=> new ReadOnlyCollectionMemberAccess(new MemberAccess(_members.Get(parameter.Metadata),
			                                                       _getter.Get(parameter.Metadata),
			                                                       _add.Get(parameter.MemberType)));
	}
}