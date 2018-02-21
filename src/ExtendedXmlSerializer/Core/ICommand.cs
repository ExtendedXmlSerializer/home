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
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive;

namespace ExtendedXmlSerializer.Core
{
	public interface ICommand : ICommand<Unit> {}

	public interface ICommand<in T>
	{
		void Execute(T parameter);
	}

	public static class CommandExtensionMethods
	{


		public static IAssignable<TKey, TValue> Assign<TKey, TValue>(this IAssignable<TKey, TValue> @this,
		                                                             TKey key, TValue value)
			=> @this.Executed(Pairs.Create(key, value)).Return(@this);

		public static ICommand<T> Executed<T>(this ICommand<T> @this, T parameter) => @this.Execute(parameter, @this);

		static TReturn Execute<T, TReturn>(this ICommand<T> @this, T parameter, TReturn @return)
		{
			@this.Execute(parameter);
			return @return;
		}

		public static T ReturnWith<TCommand, T>(this TCommand @this, T parameter) where TCommand : class, ICommand<T> => @this.Execute(parameter, parameter);

		public static ICommand<Unit> Executed(this ICommand<Unit> @this) => @this.Executed(Unit.Default);

		public static void Execute(this ICommand<Unit> @this)
		{
			@this.Execute(Unit.Default);
		}

		public static IAssignable<TKey, TValue> Assign<TKey, TValue>(this IAssignable<TKey, TValue> @this, ISource<TKey> key, TValue instance)
			=> @this.Assign(key.Get(), instance);

		/*public static IAssignable<TKey, TValue> Assigned<TKey, TValue>(this IAssignable<TKey, TValue> @this, TKey key, TValue value)
		{
			@this.Assign(key, value);
			return @this;
		}*/

		public static ICommand<T> Fold<T>(this IEnumerable<ICommand<T>> @this)
			=> new CompositeCommand<T>(@this.ToImmutableArray());

		public static ICommand<T> ToCommand<T>(this Func<ICommand<T>> @this)
			=> new DeferredCommand<T>(@this);

		public static ICommand<T> ToCommand<T>(this IParameterizedSource<T, ICommand<T>> @this) => ToCommand(@this.ToDelegate());

		public static ICommand<T> ToCommand<T>(this Func<T, ICommand<T>> @this)
			=> new DelegatedSourceCommand<T>(@this);

		public static ICommand<T> ToInstanceCommand<T>(this Func<ICommand<T>> @this)
			=> new DeferredInstanceCommand<T>(@this);

		public static ICommand<T> Fix<T>(this ICommand<T> @this, T parameter) => new FixedCommand<T>(@this, parameter);

		public static ICommand<T> ByParameter<T>(this ICommand<T> @this) where T : class
			=> new ValidatedCommand<T>(new FirstInvocationByParameterSpecification<T>(), @this);

		public static ICommand<TTo> To<TTo, TFrom>(this ICommand<TFrom> @this, A<TTo> _) => @this.To(CastCoercer<TTo, TFrom>.Default);

		public static ICommand<TTo> To<TTo, TFrom>(this ICommand<TFrom> @this, IParameterizedSource<TTo, TFrom> coercer)
			=> @this.To(coercer.ToDelegate());

		public static ICommand<TTo> To<TTo, TFrom>(
			this ICommand<TFrom> @this, Func<TTo, TFrom> coercer)
			=> new DelegatedCommand<TTo>(new CoercedCommand<TTo, TFrom>(@this.Execute, coercer).Execute);

		public static ICommand<TParameter> Unless<TParameter, TOther>(this ICommand<TParameter> @this, A<TOther> _,
		                                                              ICommand<TOther> other)
			=> @this.Unless(IsTypeSpecification<TParameter, TOther>.Default, other.To(A<TParameter>.Default));

		public static ICommand<T> Unless<T>(this ICommand<T> @this, ISpecification<T> specification,
		                                    ICommand<T> other)
			=> new ConditionalCommand<T>(specification, other, @this);
	}

	sealed class CoercedCommand<TFrom, TTo> : ICommand<TFrom>
	{
		readonly Func<TFrom, TTo>   _coercer;
		readonly Action<TTo> _source;

		public CoercedCommand(ICommand<TTo> source, IParameterizedSource<TFrom, TTo> coercer)
			: this(source.Execute, coercer.ToDelegate()) { }

		public CoercedCommand(Action<TTo> source, Func<TFrom, TTo> coercer)
		{
			_coercer = coercer;
			_source  = source;
		}

		public void Execute(TFrom parameter) => _source(_coercer(parameter));
	}

}