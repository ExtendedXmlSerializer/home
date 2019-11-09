using System;
using System.Collections;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ActivationContext : IActivationContext
	{
		readonly ITableSource<string, object> _source;
		readonly Func<object>                 _activator;
		readonly IList                        _list;

		public ActivationContext(ITableSource<string, object> source, Func<object> activator, IList list)
		{
			_source    = source;
			_activator = activator;
			_list      = list;
		}

		public bool IsSatisfiedBy(string parameter) => _source.IsSatisfiedBy(parameter);

		public object Get(string parameter) => _source.Get(parameter);

		public void Assign(string key, object value) => _source.Assign(key, value);

		public object Get() => _activator();

		public bool Remove(string key) => _source.Remove(key);

		public IEnumerator GetEnumerator() => _list.GetEnumerator();

		public void CopyTo(Array array, int index) => _list.CopyTo(array, index);

		public int Count => _list.Count;

		public bool IsSynchronized => _list.IsSynchronized;

		public object SyncRoot => _list.SyncRoot;

		public int Add(object value) => _list.Add(value is ISource<object> source ? source.Get() : value);

		public void Clear() => _list.Clear();

		public bool Contains(object value) => _list.Contains(value);

		public int IndexOf(object value) => _list.IndexOf(value);

		public void Insert(int index, object value) => _list.Insert(index, value);

		public void Remove(object value) => _list.Remove(value);

		public void RemoveAt(int index) => _list.RemoveAt(index);

		public bool IsFixedSize => _list.IsFixedSize;

		public bool IsReadOnly => _list.IsReadOnly;

		public object this[int index]
		{
			get => _list[index];
			set => _list[index] = value;
		}
	}
}