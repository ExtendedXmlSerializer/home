using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// A base object for storing values keyed on reference.
	/// </summary>
	/// <typeparam name="TKey">The key reference type.</typeparam>
	/// <typeparam name="TValue">The value type.</typeparam>
	public abstract class ReferenceCacheBase<TKey, TValue> : ITableSource<TKey, TValue> where TKey : class
	                                                                                    where TValue : class
	{
		readonly ConditionalWeakTable<TKey, TValue>                     _cache;
		readonly ConditionalWeakTable<TKey, TValue>.CreateValueCallback _callback;

		/// <inheritdoc />
		protected ReferenceCacheBase() : this(new ConditionalWeakTable<TKey, TValue>()) {}

		/// <inheritdoc />
		protected ReferenceCacheBase(ConditionalWeakTable<TKey, TValue> cache)
		{
			_cache    = cache;
			_callback = Create;
		}

		/// <exclude />
		protected abstract TValue Create(TKey parameter);

		/// <inheritdoc />
		public virtual TValue Get(TKey key) => _cache.GetValue(key, _callback);

		/// <inheritdoc />
		public virtual void Assign(TKey key, TValue value)
		{
			_cache.Remove(key);
			_cache.Add(key, value);
		}

		/// <inheritdoc />
		public bool IsSatisfiedBy(TKey parameter) => _cache.TryGetValue(parameter, out _);

		/// <inheritdoc />
		public bool Remove(TKey key) => _cache.Remove(key);
	}
}