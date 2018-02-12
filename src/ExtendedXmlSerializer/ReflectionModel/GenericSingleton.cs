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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Types;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class GenericSingleton : IGenericActivation
	{
		public static GenericSingleton Default { get; } = new GenericSingleton();
		GenericSingleton() : this(SingletonLocator.Default, typeof(SingletonLocator).GetRuntimeMethod("Get", typeof(TypeInfo).Yield().ToArray())) {}

		readonly ISingletonLocator _singletons;
		readonly MethodInfo _method;

		public GenericSingleton(ISingletonLocator singletons, MethodInfo method)
		{
			_singletons = singletons;
			_method = method;
		}

		public Expression Get(TypeInfo parameter)
		{
			var instance = Expression.Constant(_singletons);
			var call = Expression.Call(instance, _method, Expression.Constant(parameter));
			var result = Expression.Convert(call, parameter.AsType());
			return result;
		}
	}
}