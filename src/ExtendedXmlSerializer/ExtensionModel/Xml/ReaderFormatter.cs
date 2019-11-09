using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ReaderFormatter : IReaderFormatter
	{
		public static ReaderFormatter Default { get; } = new ReaderFormatter();

		ReaderFormatter() {}

		public string Get(IFormatReader parameter) => ((System.Xml.XmlReader)parameter.Get()).Name;
	}
}