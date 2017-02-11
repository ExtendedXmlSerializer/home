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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel
{
	class TypeFormatter : ITypeFormatter
	{
		public static TypeFormatter Default { get; } = new TypeFormatter();
		TypeFormatter() : this(DefaultClrDelimiters.Default.Generic, DefaultParsingDelimiters.Default.NestedClass) {}

		readonly string _nested;
		readonly char _generic;

		public TypeFormatter(char generic, string nested)
		{
			_generic = generic;
			_nested = nested;
		}

		public string Get(TypeInfo type)
			=> type.IsNested ? $"{Process(type.DeclaringType.GetTypeInfo())}{_nested}{Process(type)}" : Process(type);

		string Process(TypeInfo type) => type.IsGenericType ? type.Name.ToStringArray(_generic)[0] : type.Name;
	}
}