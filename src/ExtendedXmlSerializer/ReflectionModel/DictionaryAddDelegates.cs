using System;
using System.Collections;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class DictionaryAddDelegates : IAddDelegates
	{
		readonly Action<object, object> _action;

		public static DictionaryAddDelegates Default { get; } = new DictionaryAddDelegates();

		DictionaryAddDelegates()
		{
			_action = Add;
		}

		public Action<object, object> Get(TypeInfo parameter) => _action;

		static void Add(object dictionary, object item) => Add((IDictionary)dictionary, (DictionaryEntry)item);

		static void Add(IDictionary dictionary, DictionaryEntry entry) => dictionary.Add(entry.Key, entry.Value);
	}
}