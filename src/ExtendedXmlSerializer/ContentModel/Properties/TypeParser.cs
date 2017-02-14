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
using System.Collections.Immutable;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;

namespace ExtendedXmlSerialization.ContentModel.Properties
{
	class TypeParser : ITypeParser
	{
		readonly static Types Types = Types.Default;
		readonly static NameConverter Converter = NameConverter.Default;

		readonly ITypes _types;
		readonly INameConverter _converter;
		readonly IXmlReader _reader;

		public TypeParser(IXmlReader reader) : this(Types, Converter, reader) {}

		public TypeParser(ITypes types, INameConverter converter, IXmlReader reader)
		{
			_types = types;
			_converter = converter;
			_reader = reader;
		}

		public TypeInfo Get(string parameter)
		{
			var parsed = _converter.Parse(parameter);
			var result = Get(parsed);
			return result;
		}

		public TypeInfo Get(ParsedName name)
		{
			var identity = name.Identity;
			var key = XName.Get(identity.Name, _reader.Get(identity.Identifier).NamespaceName);
			var typeInfo = _types.Get(key);
			var arguments = name.GetArguments();
			var result = arguments.HasValue ? typeInfo.MakeGenericType(Arguments(arguments.Value)).GetTypeInfo() : typeInfo;
			return result;
		}

		Type[] Arguments(ImmutableArray<ParsedName> names)
		{
			var length = names.Length;
			var result = new Type[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = Get(names[i]).AsType();
			}
			return result;
		}
	}
}