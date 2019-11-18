using System.IO;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Component used to create a new <see cref="System.Xml.XmlReader"/> for each deserialization operation.
	/// </summary>
	public sealed class XmlReaderFactory : IXmlReaderFactory
	{
		readonly XmlParserContext  _context;
		readonly XmlReaderSettings _settings;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public XmlReaderFactory() : this(Defaults.ReaderSettings.Clone(), new NameTable().Context()) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="context"></param>
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