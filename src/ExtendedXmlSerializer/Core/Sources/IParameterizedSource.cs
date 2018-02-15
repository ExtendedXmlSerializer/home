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
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	public interface IParameterizedSource<in TParameter, out TResult>
	{
		TResult Get(TParameter parameter);
	}

	public interface IValueSource<in TParameter, out TResult> : IParameterizedSource<TParameter, TResult>, IEnumerable<TResult> {}

	public class TableValueSource<TParameter, TResult> : ValueSource<TParameter, TResult>, ISpecificationSource<TParameter, TResult>
	{
		readonly ISpecification<TParameter> _source;

		public TableValueSource()
			: this(new Dictionary<TParameter, TResult>()) {}

		public TableValueSource(IDictionary<TParameter, TResult> store)
			: this(new TableSource<TParameter, TResult>(store), new Values<TParameter, TResult>(store)) { }

		public TableValueSource(Func<TParameter, TResult> select, ConcurrentDictionary<TParameter, TResult> store)
			: this(new Cache<TParameter, TResult>(select, store), new Values<TParameter, TResult>(store)) {}

		public TableValueSource(ISpecificationSource<TParameter, TResult> source, IEnumerable<TResult> items)
			: base(source, items) => _source = source;

		public bool IsSatisfiedBy(TParameter parameter) => _source.IsSatisfiedBy(parameter);
	}

	public class ValueSource<TParameter, TResult> : IValueSource<TParameter, TResult>
	{
		readonly IParameterizedSource<TParameter, TResult> _source;
		readonly IEnumerable<TResult> _items;

		public ValueSource(IParameterizedSource<TParameter, TResult> source, IEnumerable<TResult> items)
		{
			_source = source;
			_items = items;
		}

		public TResult Get(TParameter parameter) => _source.Get(parameter);

		public IEnumerator<TResult> GetEnumerator() => _items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}