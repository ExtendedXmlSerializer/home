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
using System.Collections;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.TypeModel
{
	public static class Extensions
	{
		public static T New<T>(this IActivators @this) => New<T>(@this, typeof(T));

		public static T New<T>(this IActivators @this, Type type) => (T) @this.Get(type).Get();

		public static IEnumerator For(this IEnumeratorStore @this, object parameter)
			=> @this.Get(parameter.GetType().GetTypeInfo())?.Get((IEnumerable) parameter);

		public static bool IsSatisfiedBy(this ISpecification<TypeInfo> @this, object parameter)
			=> @this.IsSatisfiedBy(parameter.GetType().GetTypeInfo());

		public static bool IsSatisfiedBy(this ISpecification<TypeInfo> @this, Type parameter)
			=> @this.IsSatisfiedBy(parameter.GetTypeInfo());

		public static bool IsSatisfiedBy(this ISpecification<Type> @this, TypeInfo parameter)
			=> @this.IsSatisfiedBy(parameter.AsType());
	}
}