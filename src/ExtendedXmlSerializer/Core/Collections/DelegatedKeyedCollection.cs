using System;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Collections
{
	/// <summary>
	/// ATTRIBUTION: https://github.com/mattmc3/dotmore
	/// </summary>
	public class DelegatedKeyedCollection<TKey, TItem> : System.Collections.ObjectModel.KeyedCollection<TKey, TItem>
	{
		const string DelegateNullExceptionMessage = "Delegate passed cannot be null";
		readonly Func<TItem, TKey> _key;

		public DelegatedKeyedCollection(Func<TItem, TKey> key) : this(key, EqualityComparer<TKey>.Default) {}

		public DelegatedKeyedCollection(Func<TItem, TKey> key, IEqualityComparer<TKey> comparer) : base(comparer)
			=> _key = key ?? throw new ArgumentNullException(DelegateNullExceptionMessage);

		protected override TKey GetKeyForItem(TItem item) => _key(item);
	}
}