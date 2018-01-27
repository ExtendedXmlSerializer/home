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

using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class DefaultActivators : ReferenceCacheBase<Type, IActivator>, IActivators
	{
		readonly static Func<ParameterInfo, Expression> Selector = DefaultParameters.Instance.Get;

		public static DefaultActivators Default { get; } = new DefaultActivators();
		DefaultActivators() : this(ConstructorLocator.Default) {}

		readonly IConstructorLocator _locator;

		public DefaultActivators(IConstructorLocator locator) => _locator = locator;

		protected override IActivator Create(Type parameter)
		{
			var typeInfo = parameter.GetTypeInfo();
			var expression = parameter == typeof(string)
				                 ? (Expression) Expression.Default(parameter)
				                 : typeInfo.IsValueType
					                 ? Expression.New(parameter)
					                 : Reference(parameter, typeInfo);
			var convert = Expression.Convert(expression, typeof(object));
			var lambda = Expression.Lambda<Func<object>>(convert);
			var result = new Activator(lambda.Compile());
			return result;
		}

		NewExpression Reference(Type parameter, TypeInfo typeInfo)
		{
			var constructor = _locator.Get(typeInfo);
			var parameters = constructor.GetParameters();
			var result = parameters.Length > 0
				? Expression.New(constructor, parameters.Select(Selector))
				: Expression.New(parameter);
			return result;
		}

		sealed class DefaultParameters : IParameterizedSource<ParameterInfo, Expression>
		{
			readonly static IEnumerable<Expression> Initializers = Enumerable.Empty<Expression>();

			public static DefaultParameters Instance { get; } = new DefaultParameters();
			DefaultParameters() {}

			public Expression Get(ParameterInfo parameter)
				=> parameter.IsDefined(typeof(ParamArrayAttribute))
					? (Expression) Expression.NewArrayInit(
						parameter.ParameterType.GetElementType() ?? throw new InvalidOperationException("Element Type not found."), Initializers)
					: Expression.Default(parameter.ParameterType);
		}
	}
}