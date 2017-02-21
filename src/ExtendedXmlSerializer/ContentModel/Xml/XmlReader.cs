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
using System.Xml;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class XmlReader : IXmlReader
	{
		readonly System.Xml.XmlReader _reader;

		public XmlReader(System.Xml.XmlReader reader)
		{
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

		public void Reset() => _reader.MoveToElement();

		public AttributeReading? Attributes()
			=> _reader.HasAttributes ? new AttributeReading(_reader) : (AttributeReading?) null;

		public ContentReading Content()
		{
			if (_reader.HasAttributes)
			{
				switch (_reader.NodeType)
				{
					case XmlNodeType.Attribute:
						Reset();
						break;
				}
			}

			return new ContentReading(this, _reader);
		}

		public bool Contains(IIdentity identity)
			=> _reader.HasAttributes && _reader.MoveToAttribute(identity.Name, identity.Identifier);

		public string Get(string parameter) => _reader.LookupNamespace(parameter);


		public override string ToString()
			=> $"{base.ToString()}: {XmlQualifiedName.ToString(_reader.LocalName, _reader.NamespaceURI)}";

		public void Dispose() => _reader.Dispose();
	}
}