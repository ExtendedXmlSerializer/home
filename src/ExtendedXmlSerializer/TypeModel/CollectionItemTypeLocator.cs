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
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerialization.TypeModel
{
	public class CollectionItemTypeLocator : ICollectionItemTypeLocator
	{
		public static CollectionItemTypeLocator Default { get; } = new CollectionItemTypeLocator();
		CollectionItemTypeLocator() {}

		readonly static TypeInfo ArrayInfo = typeof(Array).GetTypeInfo();
		readonly static Type Type = typeof(IEnumerable<>);

		// Attribution: http://stackoverflow.com/a/17713382/3602057
		public TypeInfo Get(TypeInfo parameter)
		{
			// Type is Array
			// short-circuit if you expect lots of arrays 
			if (ArrayInfo.IsAssignableFrom(parameter))
				return parameter.GetElementType().GetTypeInfo();

			// type is IEnumerable<T>;
			if (parameter.IsGenericType && parameter.GetGenericTypeDefinition() == Type)
				return parameter.GetGenericArguments()[0].GetTypeInfo();

			// type implements/extends IEnumerable<T>;
			var result = parameter.GetInterfaces()
			                      .Where(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == Type)
			                      .Select(t => t.GenericTypeArguments[0]).FirstOrDefault()?.GetTypeInfo();
			return result;
		}
	}
}