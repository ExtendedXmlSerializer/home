using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core.Sources
{
	public abstract class ItemsBase<T> : IItems<T>
	{
		public virtual ImmutableArray<T> Get() => this.ToImmutableArray();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public abstract IEnumerator<T> GetEnumerator();
	}
}