using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	public abstract class CacheBase<TKey, TValue> : ITableSource<TKey, TValue>
	{
		readonly static EqualityComparer<TKey> EqualityComparer = EqualityComparer<TKey>.Default;

		readonly Func<TKey, TValue>                 _create;
		readonly ConcurrentDictionary<TKey, TValue> _store;

		protected CacheBase() : this(EqualityComparer) {}

		protected CacheBase(IEqualityComparer<TKey> comparer) :
			this(new ConcurrentDictionary<TKey, TValue>(comparer)) {}

		protected CacheBase(ConcurrentDictionary<TKey, TValue> store)
		{
			_store  = store;
			_create = Create;
		}

		public bool IsSatisfiedBy(TKey parameter) => _store.ContainsKey(parameter);

		protected abstract TValue Create(TKey parameter);

		public TValue Get(TKey key) => _store.GetOrAdd(key, _create);

		public void Assign(TKey key, TValue value) => _store[key] = value;

		public bool Remove(TKey key) => _store.TryRemove(key, out _);
	}
}