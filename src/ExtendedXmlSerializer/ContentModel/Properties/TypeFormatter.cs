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
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Properties
{
	class TypeFormatter : ITypeFormatter
	{
		readonly static Names Names = Names.Default;
		readonly static NameConverter Converter = NameConverter.Default;

		readonly IXmlWriter _writer;
		readonly INames _names;
		readonly INameConverter _converter;

		public TypeFormatter(IXmlWriter writer) : this(writer, Names, Converter) {}

		public TypeFormatter(IXmlWriter writer, INames names, INameConverter converter)
		{
			_writer = writer;
			_names = names;
			_converter = converter;
		}

		public string Get(TypeInfo parameter) => _converter.Format(Name(parameter));

		ParsedName Name(TypeInfo parameter)
		{
			var name = _names.Get(parameter);
			var parsed = new ParsedName(
				new Identity(name.LocalName, _writer.Get(name.NamespaceName)),
				parameter.IsGenericType ? Arguments(parameter.GetGenericArguments()) : null
			);
			return parsed;
		}

		Func<ImmutableArray<ParsedName>> Arguments(Type[] types)
		{
			var length = types.Length;
			var names = new ParsedName[length];
			for (var i = 0; i < length; i++)
			{
				names[i] = Name(types[i].GetTypeInfo());
			}
			var result = new Func<ImmutableArray<ParsedName>>(names.ToImmutableArray);
			return result;
		}
	}
}