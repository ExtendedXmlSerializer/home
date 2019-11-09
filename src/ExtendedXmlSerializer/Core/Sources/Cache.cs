using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	class Cache<TKey, TValue> : CacheBase<TKey, TValue>
	{
		readonly Func<TKey, TValue> _factory;

		public Cache(Func<TKey, TValue> factory) : this(factory, EqualityComparer<TKey>.Default) {}

		public Cache(Func<TKey, TValue> factory, IEqualityComparer<TKey> comparer)
			: this(factory, new ConcurrentDictionary<TKey, TValue>(comparer)) {}

		public Cache(Func<TKey, TValue> factory, ConcurrentDictionary<TKey, TValue> store) : base(store)
		{
			_factory = factory;
		}

		protected sealed override TValue Create(TKey parameter) => _factory(parameter);
	}
}