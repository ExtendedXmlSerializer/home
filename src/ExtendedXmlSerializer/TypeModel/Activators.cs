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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.TypeModel
{
	class Activators : ReferenceCacheBase<Type, Func<object>>, IActivators
	{
		public static Activators Default { get; } = new Activators();
		Activators() : this(ConstructorLocator.Default) {}

		readonly IConstructorLocator _locator;

		public Activators(IConstructorLocator locator)
		{
			_locator = locator;
		}

		protected override Func<object> Create(Type parameter)
		{
			var typeInfo = parameter.GetTypeInfo();
			var expression = typeInfo.IsValueType ? Expression.New(parameter) : Reference(parameter, typeInfo);
			var convert = Expression.Convert(expression, typeof(object));
			var lambda = Expression.Lambda<Func<object>>(convert);
			var result = lambda.Compile();
			return result;
		}

		NewExpression Reference(Type parameter, TypeInfo typeInfo)
		{
			var constructor = _locator.Get(typeInfo);
			var parameters = constructor.GetParameters();
			var result = parameters.Length > 0
				? Expression.New(constructor, parameters.Select(x => Expression.Default(x.ParameterType)))
				: Expression.New(parameter);
			return result;
		}
	}
}