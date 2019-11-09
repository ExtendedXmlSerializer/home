using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public static class Defaults
	{
		public static XmlWriterSettings WriterSettings { get; } = new XmlWriterSettings();

		public static XmlReaderSettings ReaderSettings { get; } = new XmlReaderSettings
		{
			IgnoreWhitespace             = true,
			IgnoreComments               = true,
			IgnoreProcessingInstructions = true
		};
	}
}