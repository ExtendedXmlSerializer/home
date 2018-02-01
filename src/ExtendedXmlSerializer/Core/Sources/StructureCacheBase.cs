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

using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.Core.Sources
{
	public abstract class StructureCacheBase<TKey, TValue> : ITableSource<TKey, TValue> where TKey : class
	                                                                                    where TValue : struct
	{
		readonly ConditionalWeakTable<TKey, Structure> _cache = new ConditionalWeakTable<TKey, Structure>();
		readonly ConditionalWeakTable<TKey, Structure>.CreateValueCallback _callback;

		protected StructureCacheBase()
		{
			_callback = CreateStructure;
		}

		Structure CreateStructure(TKey parameter) => new Structure(Create(parameter));

		protected abstract TValue Create(TKey parameter);

		public virtual TValue Get(TKey key) => _cache.GetValue(key, _callback).Item;

		public virtual void Assign(TKey key, TValue value)
		{
			_cache.Remove(key);
			_cache.Add(key, new Structure(value));
		}

		public bool IsSatisfiedBy(TKey parameter) => _cache.TryGetValue(parameter, out _);

		public bool Remove(TKey key) => _cache.Remove(key);

		class Structure
		{
			public Structure(TValue item)
			{
				Item = item;
			}

			public TValue Item { get; }
		}
	}
}