using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class Dictionaries : ReferenceCacheBase<object, IDictionary>
	{
		public static Dictionaries Default { get; } = new Dictionaries();

		Dictionaries() : this(DictionaryAdapters.Default, DictionaryPairTypesLocator.Default) {}

		readonly IGeneric<object, IDictionary> _dictionaries;
		readonly IDictionaryPairTypesLocator   _locator;

		public Dictionaries(IGeneric<object, IDictionary> dictionaries, IDictionaryPairTypesLocator locator)
		{
			_dictionaries = dictionaries;
			_locator      = locator;
		}

		protected override IDictionary Create(object parameter) => parameter as IDictionary ?? Locate(parameter);

		IDictionary Locate(object parameter)
		{
			var pair = _locator.Get(parameter.GetType());
			var result = pair.HasValue
				             ? _dictionaries.Get(pair.Value.KeyType, pair.Value.ValueType)(parameter)
				             : throw new
					               InvalidOperationException($"Could not locate dictionary from type {parameter.GetType()}.");
			return result;
		}
	}

	sealed class DictionaryAdapters : Generic<object, IDictionary>
	{
		public static DictionaryAdapters Default { get; } = new DictionaryAdapters();

		DictionaryAdapters() : base(typeof(Adapter<,>)) {}

		sealed class Adapter<TKey, TValue> : IDictionary
		{
			readonly IDictionary<TKey, TValue> _inner;

			[UsedImplicitly]
			public Adapter(IDictionary<TKey, TValue> inner) => _inner = inner;

			public void Add(object key, object value)
			{
				_inner.Add((TKey)key, (TValue)value);
			}

			public void Clear()
			{
				_inner.Clear();
			}

			public bool Contains(object key) => _inner.ContainsKey((TKey)key);

			public IDictionaryEnumerator GetEnumerator() => new Enumerator<TKey, TValue>(_inner.GetEnumerator());

			public void Remove(object key)
			{
				_inner.Remove((TKey)key);
			}

			public bool IsFixedSize => false;

			public bool IsReadOnly => _inner.IsReadOnly;

			public object this[object key]
			{
				get => _inner[(TKey)key];
				set => _inner[(TKey)key] = (TValue)value;
			}

			public ICollection Keys => (ICollection)_inner.Keys;

			public ICollection Values => (ICollection)_inner.Values;

			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_inner).GetEnumerator();

			public void CopyTo(Array array, int index)
			{
				_inner.CopyTo((KeyValuePair<TKey, TValue>[])array, index);
			}

			public int Count => _inner.Count;

			public bool IsSynchronized => false;

			public object SyncRoot => this;

			/// ATTRIBUTION: https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/DictionaryWrapper.cs
			readonly struct Enumerator<TEnumeratorKey, TEnumeratorValue> : IDictionaryEnumerator
			{
				readonly IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;

				public Enumerator(IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e) => _e = e;

				public DictionaryEntry Entry => (DictionaryEntry)Current;

				public object Key => Entry.Key;

				public object Value => Entry.Value;

				public object Current => new DictionaryEntry(_e.Current.Key, _e.Current.Value);

				public bool MoveNext() => _e.MoveNext();

				public void Reset() => _e.Reset();
			}
		}
	}
}