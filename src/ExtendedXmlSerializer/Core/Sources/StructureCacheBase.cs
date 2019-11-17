using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// A reference-based cache where the value is a struct.
	/// </summary>
	/// <typeparam name="TKey">The key type.</typeparam>
	/// <typeparam name="TValue">The value-type.</typeparam>
	public abstract class StructureCacheBase<TKey, TValue> : ITableSource<TKey, TValue> where TKey : class
	                                                                                    where TValue : struct
	{
		readonly ConditionalWeakTable<TKey, Structure> _cache =
			new ConditionalWeakTable<TKey, Structure>();
		readonly ConditionalWeakTable<TKey, Structure>.CreateValueCallback _callback;

		/// <inheritdoc />
		protected StructureCacheBase() => _callback = CreateStructure;

		Structure CreateStructure(TKey parameter) => new Structure(Create(parameter));

		/// <exclude />
		protected abstract TValue Create(TKey parameter);

		/// <inheritdoc />
		public virtual TValue Get(TKey key) => _cache.GetValue(key, _callback)
		                                             .Item;
		/// <inheritdoc />
		public virtual void Assign(TKey key, TValue value)
		{
			_cache.Remove(key);
			_cache.Add(key, new Structure(value));
		}

		/// <inheritdoc />
		public bool IsSatisfiedBy(TKey parameter) => _cache.TryGetValue(parameter, out _);

		/// <inheritdoc />
		public bool Remove(TKey key) => _cache.Remove(key);

		class Structure
		{
			public Structure(TValue item) => Item = item;

			public TValue Item { get; }
		}
	}
}