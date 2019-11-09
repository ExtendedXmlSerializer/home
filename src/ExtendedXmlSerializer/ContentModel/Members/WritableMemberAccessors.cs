using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class WritableMemberAccessors : IMemberAccessors
	{
		readonly IAllowedMemberValues _emit;
		readonly IGetterFactory       _getter;
		readonly ISetterFactory       _setter;

		[UsedImplicitly]
		public WritableMemberAccessors(IAllowedMemberValues emit) : this(emit, SetterFactory.Default) {}

		public WritableMemberAccessors(IAllowedMemberValues emit, ISetterFactory setter)
			: this(emit, GetterFactory.Default, setter) {}

		public WritableMemberAccessors(IAllowedMemberValues emit, IGetterFactory getter, ISetterFactory setter)
		{
			_emit   = emit;
			_getter = getter;
			_setter = setter;
		}

		public IMemberAccess Get(IMember parameter)
			=> new MemberAccess(_emit.Get(parameter.Metadata), _getter.Get(parameter.Metadata),
			                    _setter.Get(parameter.Metadata));
	}
}