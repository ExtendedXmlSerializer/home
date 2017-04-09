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

using System.Xml;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Conversion.Parsing;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlReaderContexts : ReferenceCacheBase<XmlNameTable, IXmlReaderContext>, IXmlReaderContexts
	{
		readonly IIdentityStore _store;
		readonly ITypes _types;
		readonly IXmlParserContexts _contexts;

		public XmlReaderContexts(IIdentityStore store, ITypes types) : this(store, types, XmlParserContexts.Default) {}

		public XmlReaderContexts(IIdentityStore store, ITypes types, IXmlParserContexts contexts)
		{
			_store = store;
			_types = types;
			_contexts = contexts;
		}

		protected override IXmlReaderContext Create(XmlNameTable parameter)
		{
			var context = _contexts.Get(parameter);
			var manager = context.NamespaceManager;
			var parser = new ReflectionParser(new TypeParser(_store, _types, new NamespaceResolver(manager)));
			var result = new XmlReaderContext(parser, manager);
			return result;
		}
	}
}