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

namespace ExtendedXmlSerializer.Core.Sources
{
	public struct Decoration<TParameter, TResult>
	{
		public Decoration(TParameter parameter, TResult result)
		{
			Parameter = parameter;
			Result = result;
		}

		public TParameter Parameter { get; }

		public TResult Result { get; }
	}

	sealed class DecorationParameterCoercer<TFrom, TTo, TResult>
		: IParameterizedSource<Decoration<TFrom, TResult>, Decoration<TTo, TResult>>
	{
		public static DecorationParameterCoercer<TFrom, TTo, TResult> Default { get; } = new DecorationParameterCoercer<TFrom, TTo, TResult>();
		DecorationParameterCoercer() : this(CastCoercer<TFrom, TTo>.Default.Get) {}

		readonly Func<TFrom, TTo> _coercer;
		public DecorationParameterCoercer(Func<TFrom, TTo> coercer) => _coercer = coercer;
		public Decoration<TTo, TResult> Get(Decoration<TFrom, TResult> parameter)
			=> new Decoration<TTo, TResult>(_coercer.Invoke(parameter.Parameter), parameter.Result);
	}

	public interface IDecoration<TParameter, TResult> : IParameterizedSource<Decoration<TParameter, TResult>, TResult> {}

	public class Decorator<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly Func<Decoration<TParameter, TResult>, TResult> _decorator;
		readonly Func<TParameter, TResult>                      _source;

		public Decorator(IParameterizedSource<Decoration<TParameter, TResult>, TResult> decorator,
		                 IParameterizedSource<TParameter, TResult> source)
			: this(decorator.Get, source.Get) {}

		public Decorator(Func<Decoration<TParameter, TResult>, TResult> decorator)
			: this(decorator, _ => default(TResult)) {}

		public Decorator(Func<Decoration<TParameter, TResult>, TResult> decorator,
		                 Func<TParameter, TResult> source)
		{
			_decorator        = decorator;
			_source      = source;
		}

		public TResult Get(TParameter parameter)
			=> _decorator(new Decoration<TParameter, TResult>(parameter, _source(parameter)));
	}

	public class ConditionalInstanceSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly Func<TParameter, bool> _specification;
		readonly TResult _true;
		readonly TResult _false;

		public ConditionalInstanceSource(ISpecification<TParameter> specification, TResult @true, TResult @false)
			: this(specification.IsSatisfiedBy, @true, @false) {}

		public ConditionalInstanceSource(Func<TParameter, bool> specification, TResult @true, TResult @false)
		{
			_specification = specification;
			_true = @true;
			_false = @false;
		}

		public TResult Get(TParameter parameter) => _specification(parameter) ? _true : _false;
	}


	public class ConditionalSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly Func<TParameter, bool> _specification;
		readonly Func<TResult, bool> _result;
		readonly Func<TParameter, TResult> _source, _fallback;

		public ConditionalSource(ISpecification<TParameter> specification, IParameterizedSource<TParameter, TResult> source,
		                         IParameterizedSource<TParameter, TResult> fallback)
			: this(specification, AlwaysSpecification<TResult>.Default, source, fallback) {}

		public ConditionalSource(ISpecification<TParameter> specification, ISpecification<TResult> result,
		                         IParameterizedSource<TParameter, TResult> source,
		                         IParameterizedSource<TParameter, TResult> fallback)
			: this(specification.IsSatisfiedBy, result.IsSatisfiedBy, source.Get, fallback.Get) {}

		public ConditionalSource(Func<TParameter, bool> specification, Func<TResult, bool> result,
		                         Func<TParameter, TResult> source)
			: this(specification, result, source, x => default(TResult)) {}

		public ConditionalSource(Func<TParameter, bool> specification, Func<TResult, bool> result,
		                         Func<TParameter, TResult> source,
		                         Func<TParameter, TResult> fallback)
		{
			_specification = specification;
			_result = result;
			_source = source;
			_fallback = fallback;
		}

		public TResult Get(TParameter parameter)
		{
			if (_specification(parameter))
			{
				var result = _source(parameter);
				if (_result(result))
				{
					return result;
				}
			}

			return _fallback(parameter);
		}
	}

	public class ConditionalSource<T> : ISource<T>
	{
		readonly Func<bool> _specification;
		readonly Func<T, bool> _result;
		readonly Func<T> _source, _fallback;

		public ConditionalSource(Func<bool> specification,
		                         ISource<T> source,
		                         ISource<T> fallback)
			: this(specification, AlwaysSpecification<T>.Default, source, fallback) {}

		public ConditionalSource(Func<bool> specification, ISpecification<T> result,
		                         ISource<T> source,
		                         ISource<T> fallback)
			: this(specification, result.IsSatisfiedBy, source.Get, fallback.Get) {}

		/*public ConditionalSource(Func<bool> specification, Func<T, bool> result,
		                         Func<T> source)
			: this(specification, result, source, () => default(T)) {}*/

		public ConditionalSource(Func<bool> specification, Func<T, bool> result,
		                         Func<T> source,
		                         Func<T> fallback)
		{
			_specification = specification;
			_result = result;
			_source = source;
			_fallback = fallback;
		}

		public T Get()
		{
			if (_specification())
			{
				var result = _source();
				if (_result(result))
				{
					return result;
				}
			}

			return _fallback();
		}
	}
}