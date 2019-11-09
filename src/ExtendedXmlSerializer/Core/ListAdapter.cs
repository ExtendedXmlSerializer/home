using System;
using System.Collections;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core
{
	sealed class ListAdapter<T> : IList
	{
		readonly ICollection<T>         _instance;
		readonly Action<object, object> _add;

		public ListAdapter(ICollection<T> instance, Action<object, object> add)
		{
			_instance = instance;
			_add      = add;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		int ICollection.Count => _instance.Count;

		object ICollection.SyncRoot => throw new NotSupportedException();

		bool ICollection.IsSynchronized => throw new NotSupportedException();

		int IList.Add(object value)
		{
			_add(_instance, value);
			return 0;
		}

		bool IList.Contains(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		int IList.IndexOf(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		object IList.this[int index]
		{
			get => throw new NotSupportedException();
			set => throw new NotSupportedException();
		}

		bool IList.IsReadOnly => throw new NotSupportedException();

		bool IList.IsFixedSize => throw new NotSupportedException();
	}
}