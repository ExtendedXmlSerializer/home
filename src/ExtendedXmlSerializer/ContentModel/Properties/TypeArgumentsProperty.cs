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
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;
using ExtendedXmlSerialization.TypeModel;
using Sprache;

namespace ExtendedXmlSerialization.ContentModel.Properties
{
	sealed class TypeArgumentsProperty : FrameworkPropertyBase<ImmutableArray<Type>>, ITypeArgumentsProperty
	{
		readonly Parser<ImmutableArray<XmlQualifiedName>> _parser;
		public static TypeArgumentsProperty Default { get; } = new TypeArgumentsProperty();
		TypeArgumentsProperty() : this(QualifiedNameListParser.Default.Get()) {}

		public TypeArgumentsProperty(Parser<ImmutableArray<XmlQualifiedName>> parser)
			: base("arguments", ImmutableArray<Type>.Empty)
		{
			_parser = parser;
		}

		protected override string Format(IXmlWriter writer, ImmutableArray<Type> instance)
			=> string.Join(",", Format(writer, instance));

		static IEnumerable<string> Format(ITypeFormatter formatter, ImmutableArray<Type> types)
		{
			for (var i = 0; i < types.Length; i++)
			{
				yield return formatter.Get(types[i].GetTypeInfo());
			}
		}

		protected override ImmutableArray<Type> Parse(IXmlReader reader, string data)
			=> Yield(reader, data).ToImmutableArray();

		IEnumerable<Type> Yield(IXmlReader reader, string data)
		{
			var names = _parser.Parse(data);
			var length = names.Length;
			for (var i = 0; i < length; i++)
			{
				yield return reader.Get(names[i].ToString()).AsType();
			}
		}
	}
}