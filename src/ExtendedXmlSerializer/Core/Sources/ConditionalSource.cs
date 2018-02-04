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

using System;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.Core.Sources
{
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
}