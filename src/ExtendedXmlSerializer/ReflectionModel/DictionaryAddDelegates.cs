using System;
using System.Collections;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class DictionaryAddDelegates : IAddDelegates
	{
		readonly Func<object, IDictionary> _dictionaries;
		readonly         Action<object, object>    _action;

		public static DictionaryAddDelegates Default { get; } = new DictionaryAddDelegates();

		DictionaryAddDelegates() : this(Dictionaries.Default.Get) {}

		DictionaryAddDelegates(Func<object, IDictionary> dictionaries)
		{
			_dictionaries = dictionaries;
			_action            = Add;
		}

		public Action<object, object> Get(TypeInfo parameter) => _action;

		void Add(object dictionary, object item) => Add(dictionary as IDictionary ?? _dictionaries(dictionary), (DictionaryEntry)item);

		static void Add(IDictionary dictionary, DictionaryEntry entry) => dictionary.Add(entry.Key, entry.Value);

	}
}