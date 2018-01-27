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
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class GenericActivation : IGenericActivation
	{
		readonly ImmutableArray<Type> _types;
		readonly ImmutableArray<ParameterExpression> _expressions;

		public GenericActivation(ImmutableArray<Type> types, ImmutableArray<ParameterExpression> expressions)
		{
			_types = types;
			_expressions = expressions;
		}

		public Expression Get(TypeInfo parameter)
		{
			var constructor = parameter.DeclaredConstructors.Only() ??
			                  parameter.GetConstructors()
			                           .Only() ??
			                  parameter.GetConstructor(_types.ToArray());
			var expressions = _expressions.ToArray();
			var types = constructor.GetParameters()
			                       .Select(x => x.ParameterType);
			var parameters = expressions.Zip(types, Defaults.ExpressionZip);
			var result = Expression.New(constructor, parameters);
			return result;
		}
	}
}