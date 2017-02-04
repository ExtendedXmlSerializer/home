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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IXmlWriter : IFormatter<ImmutableArray<Type>>, IFormatter<TypeInfo>
	{
		void Attribute(XName name, string value);
		void Write(string text);

		void Element(XName name);
		void EndCurrent();
		void Member(string name);
	}

	class XmlWriter : IXmlWriter
	{
		readonly INames _names;
		readonly System.Xml.XmlWriter _writer;

		public XmlWriter(System.Xml.XmlWriter writer) : this(Names.Default, writer) {}

		public XmlWriter(INames names, System.Xml.XmlWriter writer)
		{
			_names = names;
			_writer = writer;
		}

		public void Attribute(XName name, string value)
			=> _writer.WriteAttributeString(name.LocalName, name.NamespaceName, value);

		public string Get(ImmutableArray<Type> parameter) => string.Join(",", Generic(parameter));

		public string Get(TypeInfo parameter)
		{
			var name = QualifiedFormat(parameter);
			var result = parameter.IsGenericType ? string.Concat(name, $"[{FormatArguments(parameter)}]") : name;
			return result;
		}

		public void Write(string text) => _writer.WriteString(text);

		string FormatArguments(TypeInfo type) => Get(type.GetGenericArguments().ToImmutableArray());

		IEnumerable<string> Generic(ImmutableArray<Type> arguments)
		{
			for (var i = 0; i < arguments.Length; i++)
			{
				yield return QualifiedFormat(arguments[i].GetTypeInfo());
			}
		}

		public void Element(XName name) => _writer.WriteStartElement(name.LocalName, name.NamespaceName);

		string QualifiedFormat(TypeInfo type) => QualifiedFormat(_names.Get(type));

		string QualifiedFormat(XName name)
			=> XmlQualifiedName.ToString(name.LocalName, _writer.LookupPrefix(name.NamespaceName));

		public void EndCurrent() => _writer.WriteEndElement();
		public void Member(string name) => _writer.WriteStartElement(name);
	}
}