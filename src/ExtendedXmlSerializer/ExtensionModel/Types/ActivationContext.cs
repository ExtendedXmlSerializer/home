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

using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ActivationContext : IActivationContext
	{
		readonly ITableSource<string, object> _source;
		readonly IActivator _activator;
		readonly IList _list;

		public ActivationContext(ITableSource<string, object> source, IActivator activator, IList list)
		{
			_source = source;
			_activator = activator;
			_list = list;
		}

		public bool IsSatisfiedBy(string parameter) => _source.IsSatisfiedBy(parameter);

		public object Get(string parameter) => _source.Get(parameter);

		public void Assign(string key, object value) => _source.Assign(key, value);

		public object Get() => _activator.Get();
		public bool Remove(string key) => _source.Remove(key);

		public IEnumerator GetEnumerator() => _list.GetEnumerator();

		public void CopyTo(Array array, int index) => _list.CopyTo(array, index);

		public int Count => _list.Count;

		public bool IsSynchronized => _list.IsSynchronized;

		public object SyncRoot => _list.SyncRoot;

		public int Add(object value) => _list.Add(value);

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

		public void Execute(KeyValuePair<string, object> parameter)
		{
			_source.Execute(parameter);
		}
	}
}