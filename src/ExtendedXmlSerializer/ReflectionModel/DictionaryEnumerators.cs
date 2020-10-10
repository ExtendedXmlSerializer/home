using System;
using System.Collections;
using System.Linq;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class DictionaryEnumerators : IDictionaryEnumerators
	{
		public static DictionaryEnumerators Default { get; } = new DictionaryEnumerators();

		DictionaryEnumerators() : this(Enumerable.Empty<DictionaryEntry>()) {}

		readonly IEnumerable               _default;
		readonly Func<object, IDictionary> _dictionaries;

		public DictionaryEnumerators(IEnumerable @default) : this(@default, Dictionaries.Default.Get) {}

		public DictionaryEnumerators(IEnumerable @default, Func<object, IDictionary> dictionaries)
		{
			_default      = @default;
			_dictionaries = dictionaries;
		}

		public IEnumerator Get(IEnumerable parameter)
		{
			var dictionary = _dictionaries(parameter);
			var result     = dictionary?.GetEnumerator() ?? _default.GetEnumerator();
			return result;
		}
	}
}