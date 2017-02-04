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

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public static class Extensions
	{
		public static string FormatArguments(this ITypeNames @this, XmlWriter writer, TypeInfo type)
			=> FormatArguments(@this, writer, type.GetGenericArguments().ToImmutableArray());

		public static string FormatArguments(this ITypeNames @this, XmlWriter writer, ImmutableArray<Type> arguments)
			=> string.Join(",", Generic(@this, writer, arguments));

		public static string Format(this ITypeNames @this, XmlWriter writer, TypeInfo type)
		{
			var name = @this.QualifiedFormat(writer, type);
			var result = type.IsGenericType ? string.Concat(name, $"[{@this.FormatArguments(writer, type)}]") : name;
			return result;
		}

		static IEnumerable<string> Generic(ITypeNames @this, XmlWriter writer, ImmutableArray<Type> arguments)
		{
			for (var i = 0; i < arguments.Length; i++)
			{
				yield return @this.QualifiedFormat(writer, arguments[i].GetTypeInfo());
			}
		}

		static string QualifiedFormat(this ITypeNames @this, XmlWriter writer, TypeInfo type)
			=> writer.QualifiedFormat(@this.Get(type));

		static string QualifiedFormat(this XmlWriter @this, XName name)
			=> XmlQualifiedName.ToString(name.LocalName, @this.LookupPrefix(name.NamespaceName));
	}
}