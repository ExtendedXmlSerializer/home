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

using System.Xml.Linq;
using ExtendedXmlSerialization.ContentModel.Xml.Namespacing;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class XmlWriter : IXmlWriter
	{
		readonly System.Xml.XmlWriter _writer;
		readonly INamespaces _namespaces;
		readonly static string Xmlns = XNamespace.Xmlns.NamespaceName;

		public XmlWriter(System.Xml.XmlWriter writer, object root) : this(writer, root, new Namespaces()) {}

		public XmlWriter(System.Xml.XmlWriter writer, object root, INamespaces namespaces)
		{
			Root = root;
			_writer = writer;
			_namespaces = namespaces;
		}

		public object Root { get; }

		public void Attribute(Attribute attribute)
		{
			if (attribute.Namespace.HasValue)
			{
				_writer.WriteAttributeString(attribute.Namespace?.Name, attribute.Name, attribute.Namespace?.Identifier,
				                             attribute.Identifier);
			}
			else
			{
				_writer.WriteAttributeString(attribute.Name, attribute.Identifier);
			}
		}

		public void Element(IIdentity name) => _writer.WriteStartElement(name.Name, name.Identifier);

		public void Member(string name) => _writer.WriteStartElement(name);

		public void Write(string text) => _writer.WriteString(text);

		public void EndCurrent() => _writer.WriteEndElement();

		public void Dispose() => _writer.Dispose();

		public string Get(string identifier) => _writer.LookupPrefix(identifier) ?? Create(identifier);

		string Create(string identifier)
		{
			_writer.WriteAttributeString(_namespaces.Get(identifier).Name, Xmlns, identifier);
			return _writer.LookupPrefix(identifier);
		}
	}
}