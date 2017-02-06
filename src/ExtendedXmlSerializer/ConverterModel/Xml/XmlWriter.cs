// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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