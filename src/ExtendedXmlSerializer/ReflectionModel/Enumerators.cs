using System.Collections;
using System.Linq;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class Enumerators : IEnumerators
	{
		public static Enumerators Default { get; } = new Enumerators();

		Enumerators() : this(Enumerable.Empty<object>()) {}

		readonly IEnumerable _items;

		public Enumerators(IEnumerable items) => _items = items;

		public IEnumerator Get(IEnumerable parameter) => parameter?.GetEnumerator() ?? _items.GetEnumerator();
	}
}