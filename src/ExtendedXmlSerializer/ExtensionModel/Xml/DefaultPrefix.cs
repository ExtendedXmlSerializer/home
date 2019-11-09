namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class DefaultPrefix : IPrefix
	{
		readonly System.Xml.XmlWriter _writer;

		public DefaultPrefix(System.Xml.XmlWriter writer) => _writer = writer;

		public string Get(string parameter) => _writer.LookupPrefix(parameter ?? string.Empty);
	}
}