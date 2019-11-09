using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class MemberIdentity : IIdentity
	{
		public static MemberIdentity Default { get; } = new MemberIdentity();

		MemberIdentity() : this(new FrameworkIdentity("member")) {}

		readonly IIdentity _identity;

		public MemberIdentity(IIdentity identity) => _identity = identity;

		public string Identifier => _identity.Identifier;

		public string Name => _identity.Name;
	}
}