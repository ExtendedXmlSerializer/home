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
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	public static class Extensions
	{
		public static TypeInfo ReadType(this IXmlReader @this, XName property) => ReadType(@this, @this[property]);

		public static TypeInfo ReadType(this IXmlReader @this, string data)
		{
			var delimiters = DefaultParsingDelimiters.Default;
			var start = delimiters.GenericStart;
			var parts = data.ToStringArray(start);
			var type = @this.Type(parts[0]);
			var result = parts.Length > 1
				? @this.Generic(type,
				                string.Join(start, parts.ToArray().Skip(1))
				                      .TrimEnd(delimiters.GenericEnd)
				                      .ToStringArray(delimiters.Generics))
				: type;
			return result;
		}

		static TypeInfo Generic(this IXmlReader @this, TypeInfo definition, ImmutableArray<string> types)
		{
			var result = definition.MakeGenericType(types.Select(@this.ReadType).Select(x => x.AsType()).ToArray()).GetTypeInfo();
			return result;
		}

		static TypeInfo Type(this IXmlReader @this, string data)
		{
			var name = @this.Get(data);
			var result = @this.Get(name);
			return result;
		}

		public static string GetArguments(this IFormatter<TypeInfo> @this, TypeInfo type)
			=> @this.GetArguments(type.GetGenericArguments().ToImmutableArray());

		public static string GetArguments(this IFormatter<TypeInfo> @this, ImmutableArray<Type> types)
			=> string.Join(",", Generic(@this, types));

		static IEnumerable<string> Generic(IParameterizedSource<TypeInfo, string> formatter, ImmutableArray<Type> types)
		{
			for (var i = 0; i < types.Length; i++)
			{
				yield return formatter.Get(types[i].GetTypeInfo());
			}
		}
	}
}