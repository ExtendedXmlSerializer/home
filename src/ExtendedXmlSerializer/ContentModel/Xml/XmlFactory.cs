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
using ExtendedXmlSerializer.ContentModel.Conversion.Formatting;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	sealed class XmlFactory : IXmlFactory
	{
		readonly IIdentityStore _store;
		readonly IIdentities _identities;
		readonly IXmlReaderContexts _contexts;

		public XmlFactory(IIdentityStore store, IIdentities identities, IXmlReaderContexts contexts)
		{
			_store = store;
			_identities = identities;
			_contexts = contexts;
		}

		public IXmlWriter Create(System.Xml.XmlWriter writer, object instance)
		{
			var support = new XmlWriterSupport(writer);
			var type = new TypeInfoFormatter(_identities, support);
			var member = new MemberFormatter(type);
			var result = new XmlWriter(support, type, member, writer, instance);
			return result;
		}

		public IXmlReader Create(System.Xml.XmlReader reader)
		{
			switch (reader.MoveToContent())
			{
				case XmlNodeType.Element:
					var result = new XmlReader(_store, _contexts.Get(reader.NameTable), reader);
					return result;
				default:
					throw new InvalidOperationException($"Could not locate the content from the Xml reader '{reader}.'");
			}
		}
	}
}