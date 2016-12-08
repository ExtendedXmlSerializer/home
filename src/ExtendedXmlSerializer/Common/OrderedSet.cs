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

namespace ExtendedXmlSerialization.Common
{
	/// <summary>
	/// Attribution: http://stackoverflow.com/a/17853085/3602057
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class OrderedSet<T> : IList<T>
	{
		readonly private static T[] Items = new T[0];
		private readonly IDictionary<T, LinkedListNode<T>> _dictionary;
		private readonly LinkedList<T> _linkedList;

		public OrderedSet()
			: this(Items) {}

		public OrderedSet(params T[] items)
			: this(EqualityComparer<T>.Default, items) {}

		public OrderedSet(IEqualityComparer<T> comparer, params T[] items)
		{
			_dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
			_linkedList = new LinkedList<T>();

			foreach (var item in items)
			{
				Add(item);
			}
		}

		public int Count => _dictionary.Count;

		public virtual bool IsReadOnly => _dictionary.IsReadOnly;

		void ICollection<T>.Add(T item) => Add(item);

		public bool Add(T item)
		{
			if (_dictionary.ContainsKey(item)) return false;
			LinkedListNode<T> node = _linkedList.AddLast(item);
			_dictionary.Add(item, node);
			return true;
		}

		public void Clear()
		{
			_linkedList.Clear();
			_dictionary.Clear();
		}

		public bool Remove(T item)
		{
			LinkedListNode<T> node;
			bool found = _dictionary.TryGetValue(item, out node);
			if (!found) return false;
			_dictionary.Remove(item);
			_linkedList.Remove(node);
			return true;
		}

		public IEnumerator<T> GetEnumerator() => _linkedList.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public bool Contains(T item) => _dictionary.ContainsKey(item);

		public void CopyTo(T[] array, int arrayIndex) => _linkedList.CopyTo(array, arrayIndex);

		public int IndexOf(T item)
		{
			var count = 0;
			for (var node = _linkedList.First; node != null; node = node.Next, count++)
			{
				if (item.Equals(node.Value))
					return count;
			}
			return -1;
		}

		public void Insert(int index, T item)
		{
			if (index == 0)
			{
				_linkedList.AddFirst(item);
			}
			else
			{
				throw new NotSupportedException();
			}
			// _linkedList.AddBefore(index)
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		T IList<T>.this[int index]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
