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
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.Core.Specifications
{
	public static class Extensions
	{
		public static ISpecification<TParameter> IfAssigned<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this)
			=> @this.ToDelegate()
			        .IfAssigned();

		public static ISpecification<TParameter> IfAssigned<TParameter, TResult>(this Func<TParameter, TResult> @this)
			=> new DelegatedAssignedSpecification<TParameter, TResult>(@this);

		public static Func<T, bool> ToDelegate<T>(this ISpecification<T> @this) => @this.IsSatisfiedBy;

		public static Func<bool> Build<T>(this ISpecification<TypeInfo> @this) => @this.ToDelegate().Build(Support<T>.Key);

		public static Func<bool> Build<T>(this ISpecification<T> @this, T parameter) => @this.ToDelegate().Build(parameter);

		public static Func<bool> Fix<T>(this ISpecification<T> @this, T parameter) => @this.ToDelegate().Fix(parameter).ToDelegate();

		public static ISpecification<T> Any<T>(this ISpecification<T> @this, params T[] parameters)
			=> new AnySpecification<T>();

		public static ISpecification<T> Or<T>(this ISpecification<T> @this, params ISpecification<T>[] others)
			=> new AnySpecification<T>(@this.Yield()
			                                .Concat(others)
			                                .Fixed());

		public static ISpecification<T> And<T>(this ISpecification<T> @this, params ISpecification<T>[] others)
			=> new AllSpecification<T>(@this.Yield()
			                                .Concat(others)
			                                .Fixed());

		public static ISpecification<T> Inverse<T>(this ISpecification<T> @this) => new InverseSpecification<T>(@this);

		public static ISpecification<object> AdaptForNull<T>(this ISpecification<T> @this)
			=> new NullAwareSpecificationAdapter<T>(@this);

		public static ISpecification<object> Adapt<T>(this ISpecification<T> @this)
			=> new SpecificationAdapter<T>(@this);

		public static ISpecification<TTo> To<TFrom, TTo>(this ISpecification<TFrom> @this, A<TTo> _) => @this.To(CastCoercer<TTo, TFrom>.Default);

		public static ISpecification<TTo> To<TFrom, TTo>(this ISpecification<TFrom> @this, IParameterizedSource<TTo, TFrom> coercer)
			=> @this.To(coercer.ToDelegate());

		public static ISpecification<TTo> To<TFrom, TTo>(
			this ISpecification<TFrom> @this, Func<TTo, TFrom> coercer)
			=> new DelegatedSpecification<TTo>(new CoercedParameter<TTo, TFrom, bool>(@this.IsSatisfiedBy, coercer).Get);
	}
}