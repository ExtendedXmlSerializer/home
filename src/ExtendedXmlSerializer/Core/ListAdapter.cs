// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core
{
	sealed class ListAdapter<T> : IList
	{
		readonly ICollection<T> _instance;
		readonly Action<object, object> _add;

		public ListAdapter(ICollection<T> instance, Action<object, object> add)
		{
			_instance = instance;
			_add = add;
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