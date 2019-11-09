using System.IO;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlWriterFactory : IXmlWriterFactory
	{
		public static XmlWriterFactory Default { get; } = new XmlWriterFactory();

		XmlWriterFactory() : this(Defaults.WriterSettings) {}

		readonly XmlWriterSettings _settings;

		public XmlWriterFactory(XmlWriterSettings settings) => _settings = settings;

		public System.Xml.XmlWriter Get(Stream parameter) => System.Xml.XmlWriter.Create(parameter, _settings);

		public System.Xml.XmlWriter Get(TextWriter parameter) => System.Xml.XmlWriter.Create(parameter, _settings);
	}
}