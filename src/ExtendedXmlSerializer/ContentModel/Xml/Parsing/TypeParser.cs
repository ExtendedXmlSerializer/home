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
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ContentModel.Xml.Parsing
{
	class TypeParser : ITypeParser
	{
		public TypeParser(IXmlNamespaceResolver resolver) : this(resolver, NameParser.Default, Xml.Types.Default) {}

		readonly IXmlNamespaceResolver _resolver;
		readonly INameParser _parser;
		readonly ITypes _types;

		public TypeParser(IXmlNamespaceResolver resolver, INameParser parser, ITypes types)
		{
			_resolver = resolver;
			_parser = parser;
			_types = types;
		}

		public TypeInfo Get(string data)
		{
			var name = _parser.Get(data);
			var type = Type(name);
			var result = type.IsGenericType
				? type.MakeGenericType(Types(name.AsValid<GenericXmlQualifiedName>().Arguments).ToArray()).GetTypeInfo()
				: type;
			return result;
		}

		TypeInfo Type(XmlQualifiedName name) => _types.Get(XName.Get(name.Name, _resolver.LookupNamespace(name.Namespace)));

		IEnumerable<Type> Types(ImmutableArray<XmlQualifiedName> names)
		{
			var length = names.Length;
			for (var i = 0; i < length; i++)
			{
				yield return Type(names[i]).AsType();
			}
		}
	}
}