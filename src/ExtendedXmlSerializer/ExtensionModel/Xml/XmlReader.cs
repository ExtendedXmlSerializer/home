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

using System.Reflection;
using System.Xml;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlReader : IFormatReader
	{
		readonly IFormatReaderContext _context;
		readonly System.Xml.XmlReader _reader;
		readonly string _defaultNamespace;

		public XmlReader(IFormatReaderContext context, System.Xml.XmlReader reader)
			: this(context, reader, reader.LookupNamespace(string.Empty)) {}

		public XmlReader(IFormatReaderContext context, System.Xml.XmlReader reader, string defaultNamespace)
		{
			_context = context;
			_reader = reader;
			_defaultNamespace = defaultNamespace;
		}

		public string Name => _reader.LocalName;
		public string Identifier => _reader.NamespaceURI;

		public MemberInfo Get(string parameter) => _context.Get(parameter);

		public override string ToString() => $"{base.ToString()}: {IdentityFormatter.Default.Get(this)}";

		public bool IsSatisfiedBy(IIdentity parameter)
			=>
				_reader.HasAttributes &&
				_reader.MoveToAttribute(parameter.Name,
				                        parameter.Identifier == _defaultNamespace ? string.Empty : parameter.Identifier);

		public object Get() => _reader;

		public string Content()
		{
			switch (_reader.NodeType)
			{
				case XmlNodeType.Attribute:
					return _reader.Value;
				default:
					_reader.Read();
					var result = _reader.Value;
					_reader.Read();
					Set();
					return result;
			}
		}

		public void Set() => _reader.MoveToContent();

		public void Dispose()
		{
			_reader.Dispose();
			_context.Dispose();
		}

		public IIdentity Get(string name, string identifier) => _context.Get(name, identifier);
	}
}