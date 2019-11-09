using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.Core.Sources
{
	public abstract class StructureCacheBase<TKey, TValue> : ITableSource<TKey, TValue> where TKey : class
	                                                                                    where TValue : struct
	{
		readonly ConditionalWeakTable<TKey, Structure> _cache =
			new ConditionalWeakTable<TKey, Structure>();
		readonly ConditionalWeakTable<TKey, Structure>.CreateValueCallback _callback;

		protected StructureCacheBase()
		{
			_callback = CreateStructure;
		}

		Structure CreateStructure(TKey parameter) => new Structure(Create(parameter));

		protected abstract TValue Create(TKey parameter);

		public virtual TValue Get(TKey key) => _cache.GetValue(key, _callback)
		                                             .Item;

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