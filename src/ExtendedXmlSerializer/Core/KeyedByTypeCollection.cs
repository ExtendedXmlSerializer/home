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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#pragma warning disable 1584,1711,1572,1581,1580

namespace ExtendedXmlSerializer.Core
{
	// ATTRIBUTION: https://msdn.microsoft.com/en-us/library/ms404549%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
	/// <summary>Provides a collection whose items are types that serve as keys.</summary>
	/// <typeparam name="TItem">The item types contained in the collection that also serve as the keys for the collection.</typeparam>
	public class KeyedByTypeCollection<TItem> : KeyedCollection<Type, TItem>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.KeyedByTypeCollection`1" /> class.  </summary>
		public KeyedByTypeCollection()
			: base(null, 4) {}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.KeyedByTypeCollection`1" /> class for a specified enumeration of objects.</summary>
		/// <param name="items">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> of generic type <see cref="T:System.Object" /> used to initialize the collection.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="items" /> is null.</exception>
		public KeyedByTypeCollection(IEnumerable<TItem> items)
			: base(null, 4)
		{
			if (items == null)
				throw new ArgumentNullException(nameof(items));
			foreach (var obj in items)
				Add(obj);
		}

		/*/// <summary>Returns the first item in the collection of a specified type.</summary>
		/// <returns>The object of type <paramref name="T" /> if it is a reference type and the value of type <paramref name="T" /> if it is a value type. The default value of the type is returned if no object of type <paramref name="T" /> is contained in the collection: null if it is a reference type and 0 if it is a value type.</returns>
		/// <typeparam name="T">The type of item in the collection to find.</typeparam>
		public T Find<T>() => Find<T>(false);*/

		/// <summary>Removes an object of a specified type from the collection.</summary>
		/// <returns>The object removed from the collection.</returns>
		/// <typeparam name="T">The type of item in the collection to remove.</typeparam>
		public bool Remove<T>(T item)
		{
			for (var index = 0; index < Count; ++index)
			{
				var obj = this[index];
				var o = obj as object;
				if (o is T || ReferenceEquals(item, o))
				{
					base.Remove(obj);
					return true;
				}
			}

			return false;
		}

		public KeyedByTypeCollection<TItem> AddOrReplace<T>(T item)
		{
			if (item is TItem)
			{
				RemoveAll<T>();
				Add((TItem)(object)item);
			}
			return this;
		}

		/// <summary>Removes all of the elements of a specified type from the collection.</summary>
		/// <returns>The <see cref="T:System.Collections.ObjectModel.Collection`1" /> that contains the objects of type <paramref name="T" /> from the original collection.</returns>
		/// <typeparam name="T">The type of item in the collection to remove.</typeparam>
		public KeyedByTypeCollection<TItem> RemoveAll<T>()
		{
			this.OfType<T>()
			    .OfType<TItem>()
			    .ToArray()
			    .ForEach(base.Remove);
			return this;
		}

		/// <summary>Gets the type of an item contained in the collection.</summary>
		/// <returns>The type of the specified <paramref name="item" /> in the collection.</returns>
		/// <param name="item">The item in the collection whose type is to be retrieved.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="item" /> is null.</exception>
		protected sealed override Type GetKeyForItem(TItem item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			return item.GetType();
		}

		/// <summary>Inserts an element into the collection at a specific location.</summary>
		/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted. </param>
		/// <param name="item">The object to insert into the collection.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="item" /> is null.</exception>
		protected sealed override void InsertItem(int index, TItem item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			if (Contains(item.GetType()))
				throw new ArgumentNullException(nameof(item), $"Duplicate type: {item.GetType().FullName}");

			base.InsertItem(index, item);
		}

		/// <summary>Replaces the item at the specified index with a new object.</summary>
		/// <param name="index">The zero-based index of the <paramref name="item" /> to be replaced.</param>
		/// <param name="item">The object to add to the collection.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="item" /> is null.</exception>
		protected sealed override void SetItem(int index, TItem item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			base.SetItem(index, item);
		}
	}
}