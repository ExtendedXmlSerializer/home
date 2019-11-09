using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.Core.Sources
{
	public abstract class ReferenceCacheBase<TKey, TValue> : ITableSource<TKey, TValue> where TKey : class
	                                                                                    where TValue : class
	{
		readonly ConditionalWeakTable<TKey, TValue>                     _cache;
		readonly ConditionalWeakTable<TKey, TValue>.CreateValueCallback _callback;

		protected ReferenceCacheBase() : this(new ConditionalWeakTable<TKey, TValue>()) {}

		protected ReferenceCacheBase(ConditionalWeakTable<TKey, TValue> cache)
		{
			_cache    = cache;
			_callback = Create;
		}

		protected abstract TValue Create(TKey parameter);

		public virtual TValue Get(TKey key) => _cache.GetValue(key, _callback);

		public virtual void Assign(TKey key, TValue value)
		{
			_cache.Remove(key);
			_cache.Add(key, value);
		}

		public bool IsSatisfiedBy(TKey parameter) => _cache.TryGetValue(parameter, out _);

		public bool Remove(TKey key) => _cache.Remove(key);
	}
}