using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	class Enumerable<T> : ItemsBase<T>
	{
		readonly IEnumerable<T> _enumerable;

		public Enumerable(IEnumerable<T> enumerable) => _enumerable = enumerable;

		public override IEnumerator<T> GetEnumerator() => _enumerable.GetEnumerator();
	}
}