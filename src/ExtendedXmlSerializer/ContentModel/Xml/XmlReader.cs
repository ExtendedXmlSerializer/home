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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class XmlReader : IXmlReader
	{
		readonly static XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
		                                                      {
			                                                      IgnoreWhitespace = true,
			                                                      IgnoreComments = true,
			                                                      IgnoreProcessingInstructions = true
		                                                      };

		readonly System.Xml.XmlReader _reader;

		public XmlReader(Stream stream) : this(stream, XmlReaderSettings) {}

		public XmlReader(Stream stream, XmlReaderSettings settings) : this(System.Xml.XmlReader.Create(stream, settings)) {}

		public XmlReader(System.Xml.XmlReader reader)
		{
			_reader = reader;
			switch (_reader.MoveToContent())
			{
				case XmlNodeType.Element:
					break;
				default:
					throw new InvalidOperationException($"Could not locate the content from the Xml reader '{_reader}.'");
			}
		}

		public XName Name => XName.Get(_reader.LocalName, _reader.NamespaceURI);

		/*public TypeInfo Classification { get; }*/

		public string Value()
		{
			_reader.Read();
			var result = _reader.Value;
			_reader.Read();
			return result;
		}

		public IEnumerator<string> Members() => Items(); // Same for now...
		public IEnumerator<string> Items() => new Enumerator(_reader, _reader.Depth + 1);

		public bool Contains(XName name)
			=> _reader.HasAttributes && _reader.MoveToAttribute(name.LocalName, name.NamespaceName);

		public string this[XName name]
		{
			get
			{
				var result = _reader.Value;
				_reader.MoveToElement();
				return result;
			}
		}

		public XNamespace Get(string parameter) => XNamespace.Get(_reader.LookupNamespace(parameter));

		public override string ToString()
			=> $"{base.ToString()} - {XmlQualifiedName.ToString(_reader.LocalName, _reader.NamespaceURI)}";

		public void Dispose() => _reader.Dispose();


		sealed class Enumerator : IEnumerator<string>
		{
			readonly System.Xml.XmlReader _reader;
			readonly int _depth;

			public Enumerator(System.Xml.XmlReader reader, int depth)
			{
				_reader = reader;
				_depth = depth;
			}

			public string Current => _reader.LocalName;
			object IEnumerator.Current => _reader;

			public bool MoveNext() => _reader.Read() && _reader.IsStartElement() && _reader.Depth == _depth;

			public void Reset()
			{
				throw new NotSupportedException();
			}

			public void Dispose() => Reset();
		}
	}
}