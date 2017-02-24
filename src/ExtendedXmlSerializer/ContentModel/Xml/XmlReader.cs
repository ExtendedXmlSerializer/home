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
using System.Reflection;
using System.Xml;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class XmlReader : IXmlReader
	{
		readonly ITypeSelector _selector;
		//readonly ISource<TypeInfo> _classification;
		readonly System.Xml.XmlReader _reader;

		public XmlReader(ITypeSelector selector, System.Xml.XmlReader reader)
			: this(selector, new XmlAttributes(reader), new XmlContent(reader), reader) {}

		public XmlReader(ITypeSelector selector, IXmlAttributes attributes, IXmlContent content, System.Xml.XmlReader reader)
		{
			_selector = selector;
			Attributes = attributes;
			Content = content;
			// _classification = selector.Fix(this);
			switch (reader.MoveToContent())
			{
				case XmlNodeType.Element:
					_reader = reader;
					break;
				default:
					throw new InvalidOperationException($"Could not locate the content from the Xml reader '{reader}.'");
			}
		}

		public string Name => _reader.LocalName;
		public string Identifier => _reader.NamespaceURI;

		public TypeInfo Classification => _selector.Get(this);

		public IXmlAttributes Attributes { get; }
		public IXmlContent Content { get; }

		public string Value()
		{
			switch (_reader.NodeType)
			{
				case XmlNodeType.Attribute:
					return _reader.Value;

				default:
					_reader.Read();
					var result = _reader.Value;
					_reader.Read();
					return result;
			}
		}


		public string Get(string parameter) => _reader.LookupNamespace(parameter);


		public override string ToString()
			=> $"{base.ToString()}: {XmlQualifiedName.ToString(_reader.LocalName, _reader.NamespaceURI)}";

		public void Dispose() => _reader.Dispose();
	}
}