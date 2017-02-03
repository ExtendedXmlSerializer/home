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
using System.Runtime.CompilerServices;

namespace ExtendedXmlSerialization.Core.Sources
{
	public class WeakCache<TKey, TValue> : WeakCacheBase<TKey, TValue>
		where TKey : class where TValue : class
	{
		readonly ConditionalWeakTable<TKey, TValue>.CreateValueCallback _callback;

		public WeakCache(ConditionalWeakTable<TKey, TValue>.CreateValueCallback callback)
		{
			_callback = callback;
		}

		protected override TValue Create(TKey parameter) => _callback(parameter);
	}

	public abstract class WeakCacheBase<TKey, TValue> : IParameterizedSource<TKey, TValue> where TKey : class
	                                                                                       where TValue : class
	{
		readonly ConditionalWeakTable<TKey, TValue> _cache = new ConditionalWeakTable<TKey, TValue>();
		readonly ConditionalWeakTable<TKey, TValue>.CreateValueCallback _callback;

		protected WeakCacheBase()
		{
			_callback = Create;
		}

		protected abstract TValue Create(TKey parameter);

		public bool Contains(TKey key)
		{
			TValue temp;
			return _cache.TryGetValue(key, out temp);
		}

		public void Add(TKey key, TValue value)
		{
			_cache.Remove(key);
			_cache.Add(key, value);
		}

		public TValue Get(TKey key) => _cache.GetValue(key, _callback);
	}

	public abstract class CacheBase<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IParameterizedSource<TKey, TValue>
	{
		readonly Func<TKey, TValue> _create;

		protected CacheBase()
		{
			_create = Create;
		}

		protected abstract TValue Create(TKey parameter);

		public TValue Get(TKey key) => GetOrAdd(key, _create);
	}
}