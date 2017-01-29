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
using System.Reflection;

namespace ExtendedXmlSerialization.Core.Specifications
{
	public class IsGenericTypeSpecification : ISpecification<TypeInfo>
	{
		public static IsGenericTypeSpecification Default { get; } = new IsGenericTypeSpecification();
		IsGenericTypeSpecification() {}

		public bool IsSatisfiedBy(TypeInfo parameter) => parameter.IsGenericType;
	}

	public class IsAssignableGenericSpecification : ISpecification<TypeInfo>
	{
		readonly Type _genericType;

		public IsAssignableGenericSpecification(Type genericType)
		{
			_genericType = genericType;
		}

		// ATTRIBUTION: http://stackoverflow.com/a/5461399/3602057
		public bool IsSatisfiedBy(TypeInfo parameter)
		{
			var interfaceTypes = parameter.GetInterfaces();

			foreach (var it in interfaceTypes.Append(parameter.AsType()))
			{
				if (it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == _genericType)
					return true;
			}

			var baseType = parameter.BaseType?.GetTypeInfo();
			var result = baseType != null && IsSatisfiedBy(baseType);
			return result;
		}
	}
}