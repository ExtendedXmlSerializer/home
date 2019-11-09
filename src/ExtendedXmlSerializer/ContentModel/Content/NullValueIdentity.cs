using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class NullValueIdentity : Identity
	{
		public static NullValueIdentity Default { get; } = new NullValueIdentity();

		NullValueIdentity() : base("nil", "http://www.w3.org/2001/XMLSchema-instance") {}
	}
}