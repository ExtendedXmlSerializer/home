using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <inheritdoc />
	public class Items<T> : ItemsBase<T>
	{
		readonly ImmutableArray<T> _items;

		/// <inheritdoc />
		public Items(params T[] items) : this(items.AsEnumerable()) {}

		/// <inheritdoc />
		public Items(IEnumerable<T> items) : this(items.ToImmutableArray()) {}

		/// <inheritdoc />
		public Items(ImmutableArray<T> items) => _items = items;

		/// <inheritdoc />
		public sealed override IEnumerator<T> GetEnumerator()
		{
			var length = _items.Length;
			for (var i = 0; i < length; i++)
			{
				yield return _items[i];
			}
		}
	}
}