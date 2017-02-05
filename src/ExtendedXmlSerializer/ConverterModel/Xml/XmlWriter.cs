using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
			var name = Format(parameter);
			var result = parameter.IsGenericType
				? string.Concat(name, $"[{Get(parameter.GetGenericArguments().ToImmutableArray())}]")
				: name;
			return result;
		}

		public string Get(ImmutableArray<Type> parameter) => string.Join(",", Generic(parameter));

		string Format(TypeInfo type)
		{
			var name = _names.Get(type);
			var result = XmlQualifiedName.ToString(name.LocalName, _writer.LookupPrefix(name.NamespaceName));
			return result;
		}

		IEnumerable<string> Generic(ImmutableArray<Type> arguments)
		{
			for (var i = 0; i < arguments.Length; i++)
			{
				yield return Format(arguments[i].GetTypeInfo());
			}
		}

		public void Element(XName name) => _writer.WriteStartElement(name.LocalName, name.NamespaceName);

		public void Member(string name) => _writer.WriteStartElement(name);

		public void Write(string text) => _writer.WriteString(text);

		public void EndCurrent() => _writer.WriteEndElement();

		public void Dispose() => _writer.Dispose();
	}
}