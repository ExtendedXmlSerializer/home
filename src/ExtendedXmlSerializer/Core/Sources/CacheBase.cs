using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <exclude />
	public abstract class CacheBase<TKey, TValue> : ITableSource<TKey, TValue>
	{
		readonly Func<TKey, TValue>                 _create;
		readonly ConcurrentDictionary<TKey, TValue> _store;

		/// <exclude />
		protected CacheBase() : this(EqualityComparer<TKey>.Default) {}

		/// <exclude />
		protected CacheBase(IEqualityComparer<TKey> comparer)
			: this(new ConcurrentDictionary<TKey, TValue>(comparer)) {}

		/// <exclude />
		protected CacheBase(ConcurrentDictionary<TKey, TValue> store)
		{
			_store  = store;
			_create = Create;
		}

		/// <exclude />
		public bool IsSatisfiedBy(TKey parameter) => _store.ContainsKey(parameter);

		/// <exclude />
		protected abstract TValue Create(TKey parameter);

		/// <exclude />
		public TValue Get(TKey key) => _store.GetOrAdd(key, _create);

		/// <exclude />
		public void Assign(TKey key, TValue value) => _store[key] = value;

		/// <exclude />
		public bool Remove(TKey key) => _store.TryRemove(key, out _);
	}
}