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

using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Collections
{
	// ATTRIBUTION: https://msdn.microsoft.com/en-us/library/ms404549%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
	public class KeyedByTypeCollection<T> : DelegatedKeyedCollection<Type, T>
	{
		public KeyedByTypeCollection() : this(Enumerable.Empty<T>()) {}

		public KeyedByTypeCollection(IEnumerable<T> items) : base(InstanceTypeCoercer<T>.Default.Get)
		{
			foreach (var obj in items)
				Add(obj);
		}

		protected sealed override void InsertItem(int index, T item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			if (Contains(item.GetType()))
				throw new InvalidOperationException($"Duplicate type: {item.GetType().FullName}");

			base.InsertItem(index, item);
		}

		protected sealed override void SetItem(int index, T item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			base.SetItem(index, item);
		}
	}
}