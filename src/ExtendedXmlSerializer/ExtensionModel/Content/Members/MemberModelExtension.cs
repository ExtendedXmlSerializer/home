using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	public sealed class MemberModelExtension : ISerializerExtension
	{
		public static MemberModelExtension Default { get; } = new MemberModelExtension();

		MemberModelExtension() {}

		public IServiceRepository Get(IServiceRepository parameter) =>
			parameter.Register<IMetadataSpecification, MetadataSpecification>()
			         .Register<IValidMemberSpecification, ValidMemberSpecification>()
			         .Register<ITypeMemberSource, TypeMemberSource>()
			         .Register<ITypeMembers, TypeMembers>()
			         .Register<IMembers, ContentModel.Members.Members>()
			         .Register<IMemberAccessors, MemberAccessors>()
			         .Register<WritableMemberAccessors>()
			         .Register<ReadOnlyCollectionAccessors>()
			         .Register<VariableTypeMemberContents>()
			         .Register<DefaultMemberContents>()
			         .Register<IMemberContents, MemberContents>()
			         .Register<IMemberSerializers, MemberSerializers>()
			         .Register<IMemberSerializations, MemberSerializations>()
			         .Register<IInstanceMemberSerializations, InstanceMemberSerializations>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}