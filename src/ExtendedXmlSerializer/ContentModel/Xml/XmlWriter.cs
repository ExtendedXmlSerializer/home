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
		readonly ITypeFormatter _type;
		readonly IMemberFormatter _member;
		readonly System.Xml.XmlWriter _writer;

		public XmlWriter(IIdentityResolver resolver, ITypeFormatter type, IMemberFormatter member, System.Xml.XmlWriter writer,
		                 object root)
		{
			_resolver = resolver;
			_type = type;
			_member = member;
			_writer = writer;
			Root = root;
		}

		public object Root { get; }

		public void Attribute(Attribute attribute)
		{
			if (attribute.Namespace.HasValue)
			{
				_writer.WriteAttributeString(attribute.Namespace?.Name, attribute.Name, attribute.Namespace?.Identifier,
				                             attribute.Identifier);
			}
			else
			{
				_writer.WriteAttributeString(attribute.Name, attribute.Identifier);
			}
		}

		public void Element(IIdentity name) => _writer.WriteStartElement(name.Name, name.Identifier);

		public void Member(IIdentity member)
		{
			var identifier = member.Identifier.NullIfEmpty();
			if (identifier != null)
			{
				_writer.WriteStartElement(member.Name, member.Identifier);
			}
			else
			{
				_writer.WriteStartElement(member.Name);
			}
		}

		public void Write(string text) => _writer.WriteString(text);

		public void EndCurrent() => _writer.WriteEndElement();

		public System.Xml.XmlWriter Get() => _writer;

		public string Get(TypeInfo parameter) => _type.Get(parameter);

		public string Get(MemberInfo parameter) => parameter is TypeInfo ? Get((TypeInfo) parameter) : _member.Get(parameter);
		public string Get(string parameter) => _resolver.Get(parameter);
	}
}