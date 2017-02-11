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
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.ContentModel.Xml.Namespacing;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

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

		readonly ITypeExtractor _types;
		readonly IParameterizedSource<string, XNamespace> _prefixes;
		readonly char _namespace;
		readonly System.Xml.XmlReader _reader;

		public XmlReader(Stream stream) : this(stream, XmlReaderSettings) {}

		public XmlReader(Stream stream, XmlReaderSettings settings) : this(System.Xml.XmlReader.Create(stream, settings)) {}

		public XmlReader(System.Xml.XmlReader reader)
			: this(TypeExtractor.Default, Prefixes.Default, DefaultParsingDelimiters.Default.Namespace, reader
			) {}

		public XmlReader(ITypeExtractor types, IParameterizedSource<string, XNamespace> prefixes, char @namespace,
		                 System.Xml.XmlReader reader)
		{
			_types = types;
			_prefixes = prefixes;
			_namespace = @namespace;
			_reader = reader;
		}

		public string DisplayName => _reader.LocalName;
		public XName Name => XName.Get(_reader.LocalName, _reader.NamespaceURI);

		public TypeInfo Classification()
		{
			switch (_reader.MoveToContent())
			{
				case XmlNodeType.Element:
					return _types.Get(this);
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

		public IEnumerator Members() => Items(); // Same for now...

		public IEnumerator Items() => new Enumerator(_reader, _reader.Depth + (_reader.MoveToElement() ? 0 : 1));


		public bool Contains(XName name)
			=> _reader.HasAttributes && _reader.MoveToAttribute(name.LocalName, name.NamespaceName);

		public string this[XName name] => Contains(name) ? _reader.Value : null;

		public XName Get(string parameter)
		{
			var parts = parameter.ToStringArray(_namespace);
			var result = parts.Length == 2
				? XName.Get(parts[1], Namespace(parts[0]))
				: XName.Get(parts[0], _reader.LookupNamespace(string.Empty));
			return result;
		}

		string Namespace(string prefix) => _reader.LookupNamespace(prefix) ?? _prefixes.Get(prefix)?.NamespaceName;

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