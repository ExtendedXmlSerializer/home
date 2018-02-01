// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class SetterFactory : ISetterFactory
	{
		public static SetterFactory Default { get; } = new SetterFactory();
		SetterFactory() {}

		public Action<object, object> Get(MemberInfo parameter) => Get(parameter.DeclaringType, parameter.Name);

		static Action<object, object> Get(Type type, string name)
		{
			// Object (type object) from witch the data are retrieved
			var itemObject = Expression.Parameter(typeof(object), "item");

			// Object casted to specific type using the operator "as".
			var itemCasted = type.GetTypeInfo().IsValueType
				? Expression.Unbox(itemObject, type)
				: Expression.Convert(itemObject, type);
			// Property from casted object
			var property = itemCasted.PropertyOrField(type, name);

			// Secound parameter - value to set
			var value = Expression.Parameter(typeof(object), "value");

			// Because we use this function also for value type we need to add conversion to object
			var paramCasted = Expression.Convert(value, property.Type);

			// Assign value to property
			var assign = Expression.Assign(property, paramCasted);

			var lambda = Expression.Lambda<Action<object, object>>(assign, itemObject, value);

			var result = lambda.Compile();
			return result;
		}
	}
}