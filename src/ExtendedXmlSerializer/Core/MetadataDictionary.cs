using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.Core
{
	class MetadataDictionary<T> : IDictionary<MemberInfo, T> where T : class
	{
		readonly IDictionary<MemberInfo, T> _primary, _secondary;

		public MetadataDictionary(IDictionary<MemberInfo, T> primary)
			: this(primary, primary.ToDictionary(x => x.Key, x => x.Value, InheritedMemberComparer.Default.Get())) {}

		public MetadataDictionary(IDictionary<MemberInfo, T> primary, IDictionary<MemberInfo, T> secondary)
		{
			_primary   = primary;
			_secondary = secondary;
		}

		public IEnumerator<KeyValuePair<MemberInfo, T>> GetEnumerator() => _primary.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_primary).GetEnumerator();

		public void Add(KeyValuePair<MemberInfo, T> item)
		{
			throw new InvalidOperationException("Not supported.");
		}

		public void Clear()
		{
			throw new InvalidOperationException("Not supported.");
		}

		public bool Contains(KeyValuePair<MemberInfo, T> item) => _primary.Contains(item) || _secondary.Contains(item);

		public void CopyTo(KeyValuePair<MemberInfo, T>[] array, int arrayIndex)
		{
			_primary.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<MemberInfo, T> item) => throw new InvalidOperationException("Not supported.");

		public int Count => _primary.Count;

		public bool IsReadOnly => _primary.IsReadOnly;

		public void Add(MemberInfo key, T value)
		{
			throw new InvalidOperationException("Not supported.");
		}

		public bool ContainsKey(MemberInfo key) => _primary.ContainsKey(key) || _secondary.ContainsKey(key);

		public bool Remove(MemberInfo key) => throw new InvalidOperationException("Not supported.");

		public bool TryGetValue(MemberInfo key, out T value)
			=> _primary.TryGetValue(key, out value) || _secondary.TryGetValue(key, out value);

		public T this[MemberInfo key]
		{
			get => _primary[key] ?? _secondary[key];
			set => throw new InvalidOperationException("Not supported.");
		}

		public ICollection<MemberInfo> Keys => _primary.Keys;

		public ICollection<T> Values => _primary.Values;
	}
}