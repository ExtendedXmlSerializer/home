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
using ExtendedXmlSerializer.ContentModel.Conversion.Formatting;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	sealed class XmlWriter : IXmlWriter
	{
		readonly IIdentityResolver _resolver;
		readonly IReflectionFormatter _formatter;
		readonly System.Xml.XmlWriter _writer;

		public XmlWriter(IIdentityResolver resolver, IReflectionFormatter formatter, System.Xml.XmlWriter writer, object root)
		{
			_resolver = resolver;
			_formatter = formatter;
			_writer = writer;
			Root = root;
		}

		public object Root { get; }

		public void Start(IIdentity identity)
		{
			var identifier = identity.Identifier.NullIfEmpty();
			if (identifier != null)
			{
				_writer.WriteStartElement(identity.Name, identifier);
			}
			else
			{
				_writer.WriteStartElement(identity.Name);
			}
		}

		public void EndCurrent() => _writer.WriteEndElement();

		public System.Xml.XmlWriter Get() => _writer;

		public string Get(TypeInfo parameter) => _formatter.Get(parameter);

		public string Get(MemberInfo parameter) => parameter is TypeInfo ? Get((TypeInfo) parameter) : _formatter.Get(parameter);

		public void Content(string content) => _writer.WriteString(content);
		public void Content(IIdentity property, string content)
		{
			var identifier = property.Identifier;
			var prefix = !string.IsNullOrEmpty(identifier) ? Get(identifier) : null;
			var name = property.Name;

			if (!string.IsNullOrEmpty(prefix))
			{
				_writer.WriteAttributeString(prefix, name, identifier, content);
			}
			else
			{
				_writer.WriteAttributeString(name, content);
			}
		}

		public string Get(string parameter) => _resolver.Get(parameter);
	}
}