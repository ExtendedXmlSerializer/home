using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.ConverterModel.Xml
{
	class XmlWriter : IXmlWriter
	{
		readonly INames _names;
		readonly System.Xml.XmlWriter _writer;

		public XmlWriter(Stream stream) : this(System.Xml.XmlWriter.Create(stream)) {}
		public XmlWriter(System.Xml.XmlWriter writer) : this(Names.Default, writer) {}

		public XmlWriter(INames names, System.Xml.XmlWriter writer)
		{
			_names = names;
			_writer = writer;
		}

		public void Attribute(XName name, string value)
			=> _writer.WriteAttributeString(name.LocalName, name.NamespaceName, value);

		public string Get(TypeInfo parameter)
		{
			var name = _names.Get(parameter);
			var formatted = XmlQualifiedName.ToString(name.LocalName, _writer.LookupPrefix(name.NamespaceName));
			var result = parameter.IsGenericType ? string.Concat(formatted, $"[{this.GetArguments(parameter)}]") : formatted;
			return result;
		}

		public void Element(XName name) => _writer.WriteStartElement(name.LocalName, name.NamespaceName);

		public void Member(string name) => _writer.WriteStartElement(name);

		public void Write(string text) => _writer.WriteString(text);

		public void EndCurrent() => _writer.WriteEndElement();

		public void Dispose() => _writer.Dispose();
	}
}