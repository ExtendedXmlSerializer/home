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

using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExtendedXmlSerializer.Core.Sources
{
	public static class Extensions
	{
		public static IAssignable<TKey, TValue> Assign<TKey, TValue>(this IAssignable<TKey, TValue> @this, TKey key,
		                                                             TValue value) => @this.Executed(Pairs.Create(key, value))
		                                                                                   .Return(@this);

		public static IAlteration<object> Adapt<T>(this IAlteration<T> @this) => new AlterationAdapter<T>(@this);

		public static T Get<T>(this IParameterizedSource<Stream, T> @this, string parameter)
			=> @this.Get(new MemoryStream(Encoding.UTF8.GetBytes(parameter)));

		public static T Get<T>(this IParameterizedSource<TypeInfo, T> @this, Type parameter)
			=> @this.Get(parameter.GetTypeInfo());

		public static T Get<T>(this IParameterizedSource<Type, T> @this, TypeInfo parameter)
			=> @this.Get(parameter.AsType());

		/*public static T For<T>(this IParameterizedSource<TypeInfo, T> @this, Type parameter)
			=> @this.Get(parameter.GetTypeInfo());

		public static T For<T>(this IParameterizedSource<Type, T> @this, TypeInfo parameter)
			=> @this.Get(parameter.AsType());*/

		public static ISpecificationSource<TParameter, TTo> To<TParameter, TResult, TTo>(
			this ISpecificationSource<TParameter, TResult> @this, A<TTo> _) => @this.To(CastCoercer<TResult, TTo>.Default);

		public static ISpecificationSource<TParameter, TTo> To<TParameter, TResult, TTo>(
			this ISpecificationSource<TParameter, TResult> @this, IParameterizedSource<TResult, TTo> coercer)
			=> new SpecificationSource<TParameter, TTo>(@this, To((IParameterizedSource<TParameter, TResult>)@this, coercer));

		public static IParameterizedSource<TParameter, TTo> To<TParameter, TResult, TTo>(
			this IParameterizedSource<TParameter, TResult> @this, A<TTo> _) => @this.To(CastCoercer<TResult, TTo>.Default);

		public static IParameterizedSource<TParameter, TTo> To<TParameter, TResult, TTo>(
			this IParameterizedSource<TParameter, TResult> @this, IParameterizedSource<TResult, TTo> coercer)
			=> new CoercedResult<TParameter, TResult, TTo>(@this, coercer);

		public static IParameterizedSource<TFrom, TResult> In<TFrom, TTo, TResult>(
			this IParameterizedSource<TTo, TResult> @this, Func<TFrom, TTo> coercer) => @this.ToDelegate().In(coercer);

		public static IParameterizedSource<TFrom, TResult> In<TFrom, TTo, TResult>(
			this IParameterizedSource<TTo, TResult> @this, IParameterizedSource<TFrom, TTo> coercer)
			=> @this.ToDelegate().In(coercer.ToDelegate());

		public static IParameterizedSource<TFrom, TResult> In<TFrom, TTo, TResult>(
			this Func<TTo, TResult> @this, Func<TFrom, TTo> coercer)
			=> new CoercedParameter<TFrom, TTo, TResult>(@this, coercer);

		public static ImmutableArray<TItem>? GetAny<T, TItem>(this IParameterizedSource<T, ImmutableArray<TItem>> @this,
		                                                      T parameter)
		{
			var items = @this.Get(parameter);
			var result = items.Any() ? items : (ImmutableArray<TItem>?) null;
			return result;
		}

		public static IParameterizedSource<TParameter, TResult> If<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification)
			=> new ConditionalSource<TParameter, TResult>(specification, @this,
			                                              new FixedInstanceSource<TParameter, TResult>(default(TResult)));

		public static IParameterizedSource<TParameter, TResult> If<TParameter, TResult>(
			this TResult @this, ISpecification<TParameter> specification)
			=> new ConditionalSource<TParameter, TResult>(specification, new FixedInstanceSource<TParameter, TResult>(@this),
			                                              new FixedInstanceSource<TParameter, TResult>(default(TResult)));

		public static IParameterizedSource<TParameter, TResult> Unless<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification,
			IParameterizedSource<TParameter, TResult> other) =>
			Unless(@this, specification, AlwaysSpecification<TResult>.Default, other);

		public static IParameterizedSource<TParameter, TResult> Unless<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification,
			ISpecification<TResult> result,
			IParameterizedSource<TParameter, TResult> other)
			=> new ConditionalSource<TParameter, TResult>(specification, result, other, @this);

		public static IParameterizedSource<TParameter, TResult> Unless<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification,
			TResult other)
			=> new ConditionalSource<TParameter, TResult>(specification, new FixedInstanceSource<TParameter, TResult>(other),
			                                              @this);

		public static IParameterizedSource<TSpecification, TInstance> Unless<TSpecification, TInstance>(
			this TInstance @this, ISpecification<TSpecification> specification,
			TInstance other)
			=> new ConditionalSource<TSpecification, TInstance>(specification,
			                                                    new FixedInstanceSource<TSpecification, TInstance>(other),
			                                                    new FixedInstanceSource<TSpecification, TInstance>(@this));

		public static IParameterizedSource<TParameter, TResult> Or<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, IParameterizedSource<TParameter, TResult> next)
			where TResult : class => new LinkedDecoratedSource<TParameter, TResult>(@this, next);

		public static T Alter<T>(this IEnumerable<IAlteration<T>> @this, T seed)
			=> @this.Aggregate(seed, (current, alteration) => alteration.Get(current));

		public static TResult Alter<T, TResult>(this IEnumerable<T> @this, Func<T, TResult> alter) => @this.Select(alter).Last();

		public static Func<TResult> Build<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                       TParameter parameter)
			=> @this.ToDelegate()
			        .Build(parameter);

		public static Func<TResult> Build<TParameter, TResult>(this Func<TParameter, TResult> @this,
		                                                       TParameter parameter)
			=> @this.Fix(parameter)
			        .Singleton()
			        .Get;


		public static ISource<TResult> Fix<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> @this.ToDelegate()
			        .Fix(parameter);

		public static Func<TResult> ToDelegate<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> @this.ToDelegate()
			        .Fix(parameter)
			        .ToDelegate();

		public static ISource<TResult> Fix<TParameter, TResult>(this Func<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> new FixedSource<TParameter, TResult>(@this, parameter);

		public static ISource<T> Singleton<T>(this ISource<T> @this) => new SingletonSource<T>(@this.Get);

		public static Func<TParameter, TResult> ToDelegate<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this) => @this.Get;

		public static Func<T> ToDelegate<T>(this ISource<T> @this) => @this.Get;
	}
}