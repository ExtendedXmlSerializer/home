using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	public class TableSource<TKey, TValue> : ITableSource<TKey, TValue>
	{
		readonly IDictionary<TKey, TValue> _store;

		public TableSource() : this(new Dictionary<TKey, TValue>()) {}

		public TableSource(IEqualityComparer<TKey> comparer) : this(new Dictionary<TKey, TValue>(comparer)) {}

		public TableSource(IDictionary<TKey, TValue> store)
		{
			_store = store;
		}

		public bool IsSatisfiedBy(TKey parameter) => _store.ContainsKey(parameter);

		public virtual TValue Get(TKey parameter)
		{
			TValue result;
			return _store.TryGetValue(parameter, out result) ? result : default;
		}

		public void Assign(TKey key, TValue value) => _store[key] = value;

		public bool Remove(TKey key) => _store.Remove(key);
	}
}