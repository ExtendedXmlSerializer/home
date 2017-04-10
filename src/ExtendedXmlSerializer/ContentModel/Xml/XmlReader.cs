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
using ExtendedXmlSerializer.ContentModel.Conversion.Formatting;
using ExtendedXmlSerializer.ContentModel.Conversion.Parsing;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	sealed class XmlReader : IFormatReader
	{
		readonly IIdentityStore _store;
		readonly IXmlReaderContext _context;
		readonly string _defaultNamespace;

		public XmlReader(IIdentityStore store, IXmlReaderContext context, System.Xml.XmlReader reader)
			: this(store, context, reader, reader.LookupNamespace(string.Empty)) {}

		public XmlReader(IIdentityStore store, IXmlReaderContext context, System.Xml.XmlReader reader, string defaultNamespace)
		{
			Reader = reader;
			_store = store;
			_context = context;
			_defaultNamespace = defaultNamespace;
		}

		public System.Xml.XmlReader Reader { get; }

		public string Name => Reader.LocalName;
		public string Identifier => Reader.NamespaceURI;

		public MemberInfo Get(string parameter) => _context.Get(parameter);

		public TypeInfo Get(TypeParts parameter) => _context.Get(parameter);

		public override string ToString() => $"{base.ToString()}: {IdentityFormatter.Default.Get(this)}";

		public bool IsSatisfiedBy(IIdentity parameter)
			=>
				Reader.HasAttributes &&
				Reader.MoveToAttribute(parameter.Name,
				                       parameter.Identifier == _defaultNamespace ? string.Empty : parameter.Identifier);

		public object Get() => Reader;

		public string Content()
		{
			switch (Reader.NodeType)
			{
				case XmlNodeType.Attribute:
					return Reader.Value;
				default:
					Reader.Read();
					var result = Reader.Value;
					Reader.Read();
					Set();
					return result;
			}
		}

		public void Set() => Reader.MoveToContent();

		public IIdentity Get(IIdentity parameter)
			=> _store.Get(parameter.Name, Reader.LookupNamespace(parameter.Identifier));

		public void Dispose()
		{
			Reader.Dispose();
			_context.Dispose();
		}
	}
}