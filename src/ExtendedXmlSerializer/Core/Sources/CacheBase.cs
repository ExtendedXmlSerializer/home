// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	public abstract class CacheBase<TKey, TValue> : ITableSource<TKey, TValue>
	{
		readonly static EqualityComparer<TKey> EqualityComparer = EqualityComparer<TKey>.Default;

		readonly Func<TKey, TValue> _create;
		readonly ConcurrentDictionary<TKey, TValue> _store;

		protected CacheBase() : this(EqualityComparer) {}

		protected CacheBase(IEqualityComparer<TKey> comparer) : this(new ConcurrentDictionary<TKey, TValue>(comparer)) {}

		protected CacheBase(ConcurrentDictionary<TKey, TValue> store)
		{
			_store = store;
			_create = Create;
		}

		public bool IsSatisfiedBy(TKey parameter) => _store.ContainsKey(parameter);

		protected abstract TValue Create(TKey parameter);

		public TValue Get(TKey key) => _store.GetOrAdd(key, _create);

		public void Assign(TKey key, TValue value) => _store[key] = value;
	}
}