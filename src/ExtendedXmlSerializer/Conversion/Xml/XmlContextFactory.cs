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
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class XmlContextFactory : IXmlContextFactory
	{
		public static XmlContextFactory Default { get; } = new XmlContextFactory();
		XmlContextFactory() : this(Elements.Default) {}

		readonly IElements _elements;

		public XmlContextFactory(IElements elements)
		{
			_elements = elements;
		}

		public IWriteContext Create(XmlWriter writer, object instance)
		{
			var root = new Root(_elements.Build(instance.GetType().GetTypeInfo()));
			var result = new XmlWriteContext(writer, root);
			return result;
		}

		public IReadContext Create(Stream stream)
		{
			var text = new StreamReader(stream).ReadToEnd();
			var document = XDocument.Parse(text);
			var result = new XmlReadContext(document.Root);
			return result;
		}
	}
}