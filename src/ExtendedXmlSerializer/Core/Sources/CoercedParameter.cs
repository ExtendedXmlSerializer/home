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

namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class CoercedParameter<TFrom, TTo, TResult> : IParameterizedSource<TFrom, TResult>
	{
		readonly Func<TFrom, TTo> _coercer;
		readonly Func<TTo, TResult> _source;

		public CoercedParameter(IParameterizedSource<TTo, TResult> source, IParameterizedSource<TFrom, TTo> coercer)
			: this(source.ToDelegate(), coercer.ToDelegate()) {}

		public CoercedParameter(Func<TTo, TResult> source, Func<TFrom, TTo> coercer)
		{
			_coercer = coercer;
			_source = source;
		}

		public TResult Get(TFrom parameter) => _source(_coercer(parameter));
	}

	sealed class CoercedSource<TFrom, TTo, TResult> : IParameterizedSource<TTo, TResult>
	{
		readonly Func<TTo, TFrom> _coercer;
		readonly Func<TFrom, TResult> _source;

		public CoercedSource(IParameterizedSource<TFrom, TResult> source, IParameterizedSource<TTo, TFrom> coercer)
			: this(source.ToDelegate(), coercer.ToDelegate()) { }

		public CoercedSource(Func<TFrom, TResult> source, Func<TTo, TFrom> coercer)
		{
			_source = source;
			_coercer = coercer;
		}

		public TResult Get(TTo parameter) => _source(_coercer(parameter));
	}

}