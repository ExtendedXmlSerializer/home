using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class VerbatimContentSerializer : ISerializer<string>
	{
		public static VerbatimContentSerializer Default { get; } = new VerbatimContentSerializer();

		VerbatimContentSerializer() {}

		public string Get(IFormatReader parameter) => parameter.Content();

		public void Write(IFormatWriter writer, string instance)
		{
			writer.Verbatim(instance);
		}
	}
}