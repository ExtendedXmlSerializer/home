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

using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core.Sources
{
	class FirstAssignedSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly TResult _defaultValue;
		readonly ImmutableArray<IParameterizedSource<TParameter, TResult>> _sources;

		public FirstAssignedSource(params IParameterizedSource<TParameter, TResult>[] sources) : this(default(TResult), sources) {}

		public FirstAssignedSource(TResult defaultValue, params IParameterizedSource<TParameter, TResult>[] sources)
		{
			_defaultValue = defaultValue;
			_sources = sources.ToImmutableArray();
		}

		public TResult Get(TParameter parameter)
		{
			var length = _sources.Length;
			for (var i = 0; i < length; i++)
			{
				var result = _sources[i].Get(parameter);
				if (!Equals(result, _defaultValue))
				{
					return result;
				}
			}
			return default(TResult);
		}
	}
}