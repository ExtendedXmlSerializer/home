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
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;
using Sprache;

namespace ExtendedXmlSerialization.ContentModel.Properties
{
	sealed class ArgumentsProperty : FrameworkPropertyBase<ImmutableArray<Type>>, IArgumentsProperty
	{
		public static ArgumentsProperty Default { get; } = new ArgumentsProperty();
		ArgumentsProperty() : this(NamesList.Default) {}

		readonly Parser<IEnumerable<ParsedName>> _names;

		ArgumentsProperty(Parser<IEnumerable<ParsedName>> names) : base("arguments")
		{
			_names = names;
		}

		protected override ImmutableArray<Type> Parse(IXmlReader parameter, string data)
		{
			var items = _names.Parse(data).ToArray();
			var result = Unpack(parameter, items).ToImmutableArray();
			return result;
		}

		static Type[] Unpack(IXmlReader reader, params ParsedName[] names)
		{
			var parser = new TypeParser(reader);
			var length = names.Length;
			var result = new Type[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = parser.Get(names[i]).AsType();
			}
			return result;
		}

		protected override string Format(IXmlWriter writer, ImmutableArray<Type> instance)
			=> string.Join(",", Pack(writer, instance));

		static string[] Pack(IXmlWriter writer, ImmutableArray<Type> arguments)
		{
			var formatter = new TypeFormatter(writer);
			var length = arguments.Length;
			var result = new string[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = formatter.Get(arguments[i].GetTypeInfo());
			}
			return result;
		}
	}
}