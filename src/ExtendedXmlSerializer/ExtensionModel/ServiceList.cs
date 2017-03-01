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
using System.Collections.Immutable;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ServiceList : IServiceList
	{
		readonly IImmutableList<object> _services;

		public ServiceList(IImmutableList<object> services)
		{
			_services = services;
		}

		public IEnumerator<object> GetEnumerator() => _services.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _services.GetEnumerator();

		public int Count => _services.Count;

		public object this[int index] => _services[index];

		public IImmutableList<object> Clear() => _services.Clear();

		public IImmutableList<object> Add(object value) => _services.Add(value);

		public IImmutableList<object> Replace(object oldValue, object newValue, IEqualityComparer<object> equalityComparer)
			=> _services.Replace(oldValue, newValue, equalityComparer);

		public IImmutableList<object> SetItem(int index, object value) => _services.SetItem(index, value);

		public IImmutableList<object> RemoveAt(int index) => _services.RemoveAt(index);

		public IImmutableList<object> RemoveRange(int index, int count) => _services.RemoveRange(index, count);

		public IImmutableList<object> RemoveRange(IEnumerable<object> items, IEqualityComparer<object> equalityComparer)
			=> _services.RemoveRange(items, equalityComparer);

		public IImmutableList<object> RemoveAll(Predicate<object> match) => _services.RemoveAll(match);

		public IImmutableList<object> Remove(object value, IEqualityComparer<object> equalityComparer)
			=> _services.Remove(value, equalityComparer);

		public IImmutableList<object> InsertRange(int index, IEnumerable<object> items) => _services.InsertRange(index, items);

		public IImmutableList<object> Insert(int index, object element) => _services.Insert(index, element);

		public IImmutableList<object> AddRange(IEnumerable<object> items) => _services.AddRange(items);

		public int LastIndexOf(object item, int index, int count, IEqualityComparer<object> equalityComparer)
			=> _services.LastIndexOf(item, index, count, equalityComparer);

		public int IndexOf(object item, int index, int count, IEqualityComparer<object> equalityComparer)
			=> _services.IndexOf(item, index, count, equalityComparer);

		public object GetService(Type serviceType)
			=> new ServiceProvider(_services.ToImmutableArray()).GetService(serviceType);
	}
}