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
	public class TableSource<TKey, TValue> : ITableSource<TKey, TValue>
	{
		readonly IDictionary<TKey, TValue> _store;

		public TableSource() : this(new Dictionary<TKey, TValue>()) {}

		public TableSource(IDictionary<TKey, TValue> store)
		{
			_store = store;
		}

		public bool IsSatisfiedBy(TKey parameter) => _store.ContainsKey(parameter);

		public virtual TValue Get(TKey parameter)
		{
			TValue result;
			return _store.TryGetValue(parameter, out result) ? result : default(TValue);
		}

		public void Assign(TKey key, TValue value) => _store[key] = value;
		public bool Remove(TKey key) => _store.Remove(key);
	}
}