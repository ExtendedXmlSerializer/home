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

namespace ExtendedXmlSerialization.Core.Sources
{
	public static class Extensions
	{
		public static IParameterizedSource<TParameter, TResult> Or<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, IParameterizedSource<TParameter, TResult> next)
			where TResult : class => new LinkedDecoratedSource<TParameter, TResult>(@this, next);

		public static T Alter<T>(this IEnumerable<IAlteration<T>> @this, T seed)
			=> @this.Aggregate(seed, (current, alteration) => alteration.Get(current));

		public static Func<TResult> Build<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                       TParameter parameter)
			=> @this.Fix(parameter).Singleton().Get;

		public static ISource<TResult> Fix<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> new FixedSource<TParameter, TResult>(@this, parameter);

		public static ISource<T> Singleton<T>(this ISource<T> @this) => new SingletonSource<T>(@this.Get);

		public static Func<TParameter, TResult> ToDelegate<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this) => @this.Get;

		public static Func<T> ToDelegate<T>(this ISource<T> @this) => @this.Get;
	}
}