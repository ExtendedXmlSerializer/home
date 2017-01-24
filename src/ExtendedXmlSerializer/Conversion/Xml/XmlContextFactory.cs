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
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class XmlContextFactory : IXmlContextFactory
	{
		readonly IElements _elements;
		readonly INamespaces _namespaces;
		readonly IXmlReadContextFactory _factory;
		readonly ITypes _types;

		public XmlContextFactory(IElements elements, INamespaces namespaces, ITypes types)
			: this(elements, namespaces, new XmlReadContextFactory(elements, types, new NameConverter(namespaces)), types) {}

		public XmlContextFactory(IElements elements, INamespaces namespaces, IXmlReadContextFactory factory, ITypes types)
		{
			_elements = elements;
			_namespaces = namespaces;
			_factory = factory;
			_types = types;
		}

		public IWriteContext Create(XmlWriter writer, object instance)
		{
			var element = _elements.Get(instance.GetType().GetTypeInfo());
			var root = new Root(element);
			var result = new XmlWriteContext(_namespaces, _elements, writer, root);
			return result;
		}

		public IReadContext Create(Stream stream)
		{
			var text = new StreamReader(stream).ReadToEnd();
			var document = XDocument.Parse(text).Root;
			var element = _elements.Get(_types.Get(document.Name));
			var result = new XmlReadContext(_factory, element, document);
			return result;
		}
	}
}