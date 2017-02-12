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

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.ContentModel.Xml.Namespacing;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class XmlWriter : TypeQualifiedNameFormatter, IXmlWriter
	{
		readonly IXmlNamespaceResolver _resolver;
		readonly System.Xml.XmlWriter _writer;

		public XmlWriter(System.Xml.XmlWriter writer) : this(new Resolver(writer), writer) {}

		public XmlWriter(IXmlNamespaceResolver resolver, System.Xml.XmlWriter writer) : base(resolver)
		{
			_resolver = resolver;
			_writer = writer;
		}

		public void Attribute(XName name, string value)
			=>
				_writer.WriteAttributeString(_resolver.LookupPrefix(name.NamespaceName), name.LocalName, name.NamespaceName, value);


		public void Element(XName name) => _writer.WriteStartElement(name.LocalName, name.NamespaceName);

		public void Member(string name) => _writer.WriteStartElement(name);

		public void Write(string text) => _writer.WriteString(text);

		public void EndCurrent() => _writer.WriteEndElement();

		public void Dispose() => _writer.Dispose();

		sealed class Resolver : IXmlNamespaceResolver
		{
			readonly IPrefixes _prefixes;
			readonly System.Xml.XmlWriter _writer;

			public Resolver(System.Xml.XmlWriter writer) : this(Prefixes.Default, writer) {}

			public Resolver(IPrefixes prefixes, System.Xml.XmlWriter writer)
			{
				_prefixes = prefixes;
				_writer = writer;
			}

			public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
			{
				throw new NotSupportedException();
			}

			public string LookupNamespace(string prefix)
			{
				throw new NotSupportedException();
			}

			public string LookupPrefix(string namespaceName) => _writer.LookupPrefix(namespaceName) ?? Create(namespaceName);

			string Create(string @namespace)
			{
				_writer.WriteAttributeString(_prefixes.Get(XNamespace.Get(@namespace)), XNamespace.Xmlns.NamespaceName, @namespace);
				return _writer.LookupPrefix(@namespace);
			}
		}
	}
}