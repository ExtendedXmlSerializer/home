using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class DictionaryEnumerators : IDictionaryEnumerators
	{
		public static DictionaryEnumerators Default { get; } = new DictionaryEnumerators();

		DictionaryEnumerators() : this(Enumerable.Empty<DictionaryEntry>()) {}

		readonly IEnumerable<DictionaryEntry> _entries;

		public DictionaryEnumerators(IEnumerable<DictionaryEntry> entries) => _entries = entries;

		public IEnumerator Get(IEnumerable parameter) => (IEnumerator)((IDictionary)parameter)?.GetEnumerator()
		                                                 ?? _entries.GetEnumerator();
	}
}