using System.IO;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public sealed class XmlReaderFactory : IXmlReaderFactory
	{
		readonly XmlParserContext  _context;
		readonly XmlReaderSettings _settings;

		public XmlReaderFactory() : this(Defaults.ReaderSettings.Clone(), new NameTable().Context()) {}

		public XmlReaderFactory(XmlReaderSettings settings, XmlParserContext context)
		{
			_context  = context;
			_settings = settings;
		}

		public System.Xml.XmlReader Get(Stream parameter)
			=> System.Xml.XmlReader.Create(parameter, _settings, _context);

		public System.Xml.XmlReader Get(TextReader parameter)
			=> System.Xml.XmlReader.Create(parameter, _settings, _context);
	}
}