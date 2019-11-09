using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class NullElementIdentity : IIdentity
	{
		public static NullElementIdentity Default { get; } = new NullElementIdentity();

		NullElementIdentity() : this(new FrameworkIdentity("Null")) {}

		readonly IIdentity _identity;

		public NullElementIdentity(IIdentity identity) => _identity = identity;

		public string Identifier => _identity.Identifier;

		public string Name => _identity.Name;
	}
}