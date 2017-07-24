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
using System.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class DictionaryPairTypesLocator : IDictionaryPairTypesLocator
	{
		public static DictionaryPairTypesLocator Default { get; } = new DictionaryPairTypesLocator();
		DictionaryPairTypesLocator() : this(typeof(IDictionary<,>)) {}

		readonly Type _type;

		public DictionaryPairTypesLocator(Type type) => _type = type;

		public DictionaryPairTypes Get(TypeInfo parameter)
		{
			foreach (var it in parameter.GetInterfaces().ToMetadata().Add(parameter))
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == _type)
				{
					var arguments = it.GetGenericArguments();
					var mapping = new DictionaryPairTypes(arguments[0].GetTypeInfo(), arguments[1].GetTypeInfo());
					return mapping;
				}
			}

			var baseType = parameter.BaseType?.GetTypeInfo();
			var result = baseType != null ? Get(baseType) : null;
			return result;
		}
	}
}