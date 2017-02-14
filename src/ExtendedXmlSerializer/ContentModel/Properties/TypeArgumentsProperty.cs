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

using System.Collections.Generic;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;
using ExtendedXmlSerialization.Core;
using Sprache;

namespace ExtendedXmlSerialization.ContentModel.Properties
{
	public interface IQualifiedNameArgumentsProperty : IProperty<IEnumerable<QualifiedName>> {}

	sealed class TypeArgumentsProperty : FrameworkPropertyBase<IEnumerable<QualifiedName>>, IQualifiedNameArgumentsProperty
	{
		readonly IQualifiedNameProperty _property;
		readonly Parser<IEnumerable<QualifiedName>> _parser;
		public static TypeArgumentsProperty Default { get; } = new TypeArgumentsProperty();
		TypeArgumentsProperty() : this(TypeProperty.Default, QualifiedNameListParser.Default.Get()) {}

		public TypeArgumentsProperty(IQualifiedNameProperty property, Parser<IEnumerable<QualifiedName>> parser) : base("arguments")
		{
			_property = property;
			_parser = parser;
		}

		public override string Format(IEnumerable<QualifiedName> instance) => string.Join(",", FormatNames(instance.Fixed()));

		string[] FormatNames(QualifiedName[] names)
		{
			var length = names.Length;
			var result = new string[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = _property.Format(names[i]);
			}
			return result;
		}

		public override IEnumerable<QualifiedName> Parse(string data) => _parser.Parse(data);
	}
}