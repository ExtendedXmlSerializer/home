using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class MemberProperty : DelegatedProperty<bool>
	{
		public static MemberProperty Default { get; } = new MemberProperty();

		MemberProperty() : base(_ => true, _ => string.Empty, MemberIdentity.Default) {}
	}
}