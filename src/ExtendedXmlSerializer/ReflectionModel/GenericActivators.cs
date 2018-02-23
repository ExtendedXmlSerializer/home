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

using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class GenericActivators<T> : IParameterizedSource<TypeInfo, T>
	{
		readonly IGenericActivation _activation;
		readonly ImmutableArray<ParameterExpression> _expressions;

		public GenericActivators(params Type[] types) : this(types.ToImmutableArray()) {}

		public GenericActivators(ImmutableArray<Type> types)
			: this(types, types.Select(Defaults.Parameter)
			                   .ToImmutableArray()) {}

		public GenericActivators(ImmutableArray<Type> types, ImmutableArray<ParameterExpression> expressions)
			: this(new GenericActivation(types, expressions), expressions) {}

		public GenericActivators(IGenericActivation activation) : this(
		                                                               activation,
		                                                               ImmutableArray<ParameterExpression>.Empty) {}

		public GenericActivators(IGenericActivation activation, ImmutableArray<ParameterExpression> expressions)
		{
			_activation = activation;
			_expressions = expressions;
		}

		public T Get(TypeInfo parameter) => Expression.Lambda<T>(_activation.Get(parameter), _expressions.ToArray())
		                                              .Compile();
	}
}