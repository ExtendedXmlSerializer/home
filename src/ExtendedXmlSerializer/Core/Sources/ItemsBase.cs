using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// Provides a general purpose container to resolve an instance of items.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	public abstract class ItemsBase<T> : IItems<T>
	{
		/// <inheritdoc />
		public virtual ImmutableArray<T> Get() => this.ToImmutableArray();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <inheritdoc />
		public abstract IEnumerator<T> GetEnumerator();
	}
}