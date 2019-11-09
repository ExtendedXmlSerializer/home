using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class Identity<T> : IWriter<T>
	{
		readonly IIdentity _identity;

		public Identity(IIdentity identity) => _identity = identity;

		public void Write(IFormatWriter writer, T _) => writer.Start(_identity);
	}
}