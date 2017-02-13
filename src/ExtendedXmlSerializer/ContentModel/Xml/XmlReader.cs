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
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class XmlReader : IXmlReader
	{
		readonly static Func<string, XmlQualifiedName> Parser = NameParser.Default.Cache<string, XmlQualifiedName>().Get;

		readonly static XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
		                                                      {
			                                                      IgnoreWhitespace = true,
			                                                      IgnoreComments = true,
			                                                      IgnoreProcessingInstructions = true
		                                                      };

		readonly Func<string, XmlQualifiedName> _parser;
		readonly ITypes _types;
		readonly System.Xml.XmlReader _reader;

		public XmlReader(Stream stream) : this(Parser, Xml.Types.Default, System.Xml.XmlReader.Create(stream, XmlReaderSettings)) {}

		public XmlReader(Func<string, XmlQualifiedName> parser, ITypes types, System.Xml.XmlReader reader)
		{
			_parser = parser;
			_types = types;
			_reader = reader;
			switch (_reader.MoveToContent())
			{
				case XmlNodeType.Element:
					break;
				default:
					throw new InvalidOperationException($"Could not locate the content from the Xml reader '{_reader}.'");
			}
		}

		public string DisplayName => _reader.LocalName;
		public XName Name => XName.Get(_reader.LocalName, _reader.NamespaceURI);

		public string Value()
		{
			_reader.Read();
			var result = _reader.Value;
			_reader.Read();
			return result;
		}

		public IEnumerator Members() => Items(); // Same for now...
		public IEnumerator Items() => new Enumerator(_reader, _reader.Depth + 1);

		public string this[XName name] => _reader.HasAttributes ? _reader.GetAttribute(name.LocalName, name.NamespaceName) : null;

		public TypeInfo Get(string data)
		{
			var name = _parser(data);
			var type = Type(name);
			var result = type.IsGenericType
				? type.MakeGenericType(Types(name.AsValid<GenericXmlQualifiedName>().Arguments).ToArray()).GetTypeInfo()
				: type;
			return result;
		}

		string LookupNamespace(string prefix) => _reader.LookupNamespace(prefix) ?? _reader.LookupNamespace(string.Empty);

		TypeInfo Type(XmlQualifiedName name) => _types.Get(XName.Get(name.Name, LookupNamespace(name.Namespace)));

		IEnumerable<Type> Types(ImmutableArray<XmlQualifiedName> names)
		{
			var length = names.Length;
			for (var i = 0; i < length; i++)
			{
				yield return Type(names[i]).AsType();
			}
		}

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