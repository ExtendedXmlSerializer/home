// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
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
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.ConverterModel.Properties;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ConverterModel.Xml
{
	class XmlReader : IXmlReader
	{
		readonly static XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
		                                                      {
			                                                      IgnoreWhitespace = true,
			                                                      IgnoreComments = true,
			                                                      IgnoreProcessingInstructions = true
		                                                      };

		readonly ITypes _types;
		readonly IParsingDelimiters _delimiters;
		readonly System.Xml.XmlReader _reader;
		readonly static TypeInfo ArrayType = typeof(Array).GetTypeInfo();

		public XmlReader(Stream stream) : this(stream, XmlReaderSettings) {}
		public XmlReader(Stream stream, XmlReaderSettings settings) : this(System.Xml.XmlReader.Create(stream, settings)) {}
		public XmlReader(System.Xml.XmlReader reader) : this(Types.Default, DefaultParsingDelimiters.Default, reader) {}

		public XmlReader(ITypes types, IParsingDelimiters delimiters, System.Xml.XmlReader reader)
		{
			_types = types;
			_delimiters = delimiters;
			_reader = reader;
		}

		public string DisplayName => _reader.LocalName;
		public TypeInfo Classification()
		{
			switch (_reader.MoveToContent())
			{
				case XmlNodeType.Element:
					var name = XName.Get(_reader.LocalName, _reader.NamespaceURI);
					var result = Get(name);
					if (Equals(result, ArrayType))
					{
						return ItemTypeProperty.Default.Get(this).MakeArrayType().GetTypeInfo();
					}
					return result;
			}
			throw new InvalidOperationException($"Could not locate the type from the current Xml reader '{_reader}.'");
		}

		public string Value()
		{
			_reader.Read();
			var result = _reader.Value;
			_reader.Read();
			return result;
		}

		public IEnumerator Members() => new Enumerator(_reader, _reader.Depth + 1);

		public IEnumerator Items() => new Enumerator(_reader, _reader.Depth + 1);

		public string this[XName name] => _reader.GetAttribute(name.LocalName, name.NamespaceName);

		public XName Get(string parameter)
		{
			var parts = parameter.ToStringArray(_delimiters.Namespace);
			var result = parts.Length == 2 ? XName.Get(parts[1], _reader.LookupNamespace(parts[0])) : XName.Get(parts[0], _reader.LookupNamespace(string.Empty));
			return result;
		}

		public TypeInfo Get(XName parameter) => _types.Get(parameter);

		public void Dispose() => _reader.Dispose();

		sealed class Enumerator : IEnumerator
		{
			readonly System.Xml.XmlReader _reader;
			readonly int _depth;

			public Enumerator(System.Xml.XmlReader reader, int depth)
			{
				_reader = reader;
				_depth = depth;
			}

			public object Current => _reader;

			public bool MoveNext() => _reader.Read() && _reader.IsStartElement() && _reader.Depth == _depth;

			public void Reset()
			{
				throw new NotSupportedException();
			}
		}
	}
}