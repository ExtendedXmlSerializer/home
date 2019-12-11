using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	/// <summary>
	/// Default serializer extension that configures the member model for selecting appropriate serializers for members, as
	/// well as accessing member values.
	/// </summary>
	public sealed class MemberModelExtension : ISerializerExtension
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static MemberModelExtension Default { get; } = new MemberModelExtension();

		MemberModelExtension() {}

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IMetadataSpecification, MetadataSpecification>()
			            .Register<IValidMemberSpecification, ValidMemberSpecification>()
			            .Register<ITypeMemberSource, TypeMemberSource>()
			            .Register<ITypeMembers, TypeMembers>()
			            .Register<IMembers, ContentModel.Members.Members>()
			            .Register<IMemberAccessors, MemberAccessors>()
			            .Register<WritableMemberAccessors>()
			            .Register<ReadOnlyCollectionAccessors>()
			            .RegisterWithDependencies<IMemberContentsCore, MemberContentsCore>()
			            .RegisterWithDependencies<IMemberContents, MemberContents>()
			            .Register<IMemberSerializers, MemberSerializers>()
			            .Register<IMemberSerializations, MemberSerializations>()
			            .Register<IInstanceMemberSerializations, InstanceMemberSerializations>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}