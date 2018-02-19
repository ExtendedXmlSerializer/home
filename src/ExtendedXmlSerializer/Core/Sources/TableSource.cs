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

using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	class DuplexTable<TKey, TValue> : DecoratedTable<TKey, TValue>, IParameterizedSource<TValue, TKey>
	{
		readonly ITableSource<TValue, TKey> _other;

		public DuplexTable() : this(new TableSource<TKey, TValue>(), new TableSource<TValue, TKey>()) {}

		public DuplexTable(ITableSource<TKey, TValue> source, ITableSource<TValue, TKey> other)
			: base(new DuplexTableStore<TKey, TValue>(source, other)) => _other = other;

		public TKey Get(TValue parameter) => _other.Get(parameter);
	}

	class DuplexTableStore<TKey, TValue> : ITableSource<TKey, TValue>
	{
		readonly ITableSource<TKey, TValue> _source;
		readonly ITableSource<TValue, TKey> _other;

		public DuplexTableStore(ITableSource<TKey, TValue> source, ITableSource<TValue, TKey> other)
		{
			_source = source;
			_other  = other;
		}

		public bool IsSatisfiedBy(TKey parameter) => _source.IsSatisfiedBy(parameter);

		public TValue Get(TKey parameter) => _source.Get(parameter);

		public void Execute(KeyValuePair<TKey, TValue> parameter)
		{
			_source.Execute(parameter);
			_other.Assign(parameter.Value, parameter.Key);
		}

		public bool Remove(TKey key) => _source.Remove(key);

		public TKey Get(TValue parameter) => _other.Get(parameter);
	}

	public class TableSource<TKey, TValue> : ITableSource<TKey, TValue>
	{
		readonly IDictionary<TKey, TValue> _store;

		public TableSource() : this(new Dictionary<TKey, TValue>()) {}

		public TableSource(IEqualityComparer<TKey> comparer) : this(new Dictionary<TKey, TValue>(comparer)) {}

		public TableSource(IDictionary<TKey, TValue> store) => _store = store;

		public bool IsSatisfiedBy(TKey parameter) => _store.ContainsKey(parameter);

		public TValue Get(TKey parameter) => _store.TryGetValue(parameter, out var result) ? result : default(TValue);

		public void Assign(TKey key, TValue value) => _store[key] = value;
		public bool Remove(TKey key) => _store.Remove(key);
		public void Execute(KeyValuePair<TKey, TValue> parameter)
		{
			_store[parameter.Key] = parameter.Value;
		}
	}
}