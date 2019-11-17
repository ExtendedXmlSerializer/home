using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <inheritdoc />
	public class TableSource<TKey, TValue> : ITableSource<TKey, TValue>
	{
		readonly IDictionary<TKey, TValue> _store;

		/// <inheritdoc />
		public TableSource() : this(new Dictionary<TKey, TValue>()) {}

		/// <inheritdoc />
		public TableSource(IEqualityComparer<TKey> comparer) : this(new Dictionary<TKey, TValue>(comparer)) {}

		/// <inheritdoc />
		public TableSource(IDictionary<TKey, TValue> store) => _store = store;

		/// <inheritdoc />
		public bool IsSatisfiedBy(TKey parameter) => _store.ContainsKey(parameter);

		/// <inheritdoc />
		public virtual TValue Get(TKey parameter)
		{
			TValue result;
			return _store.TryGetValue(parameter, out result) ? result : default;
		}

		/// <inheritdoc />
		public void Assign(TKey key, TValue value) => _store[key] = value;

		/// <inheritdoc />
		public bool Remove(TKey key) => _store.Remove(key);
	}
}