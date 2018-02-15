// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Collections
{
	public class Group<T> : IGroup<T>
	{
		readonly IList<T> _collection;
		public Group(GroupName name) : this(name, Enumerable.Empty<T>()) {}

		public Group(GroupName name, params T[] items) : this(name, items.AsEnumerable()) {}

		public Group(GroupName name, IEnumerable<T> items) : this(name, items.ToList()) {}

		public Group(GroupName name, IList<T> collection)
		{
			Name = name;
			_collection = collection;
		}

		public GroupName Name { get; }

		public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(T item)
		{
			_collection.Add(item);
		}

		public void Clear()
		{
			_collection.Clear();
		}

		public bool Contains(T item) => _collection.Contains(item);

		public void CopyTo(T[] array, int arrayIndex)
		{
			_collection.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item) => _collection.Remove(item);

		public int Count => _collection.Count;

		public bool IsReadOnly => _collection.IsReadOnly;
		public int IndexOf(T item) => _collection.IndexOf(item);

		public void Insert(int index, T item)
		{
			_collection.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_collection.RemoveAt(index);
		}

		public T this[int index]
		{
			get => _collection[index];
			set => _collection[index] = value;
		}
	}
}