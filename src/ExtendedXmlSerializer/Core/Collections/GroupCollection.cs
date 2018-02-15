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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Collections
{
	sealed class SortAlteration<T> : OrderByAlteration<T, T>
	{
		public static SortAlteration<T> Default { get; } = new SortAlteration<T>();
		SortAlteration() : base(Self<T>.Default.Get, SortComparer<T>.Default) {}
	}

	public class GroupCollection<T> : DelegatedSource<GroupName, IList<T>>, IGroupCollection<T>
	{
		readonly IOrderedDictionary<GroupName, IList<T>> _store;
		readonly Func<IEnumerable<T>, IEnumerable<T>> _select;

		public GroupCollection(IEnumerable<IGroup<T>> groups)
			: this(groups, GroupPairs<T>.Default) {}

		public GroupCollection(IEnumerable<IGroup<T>> groups, IGroupPairs<T> pairs)
			: this(new OrderedDictionary<GroupName, IList<T>>(groups.Select(pairs.Get)), SortAlteration<T>.Default.Get) {}

		public GroupCollection(IOrderedDictionary<GroupName, IList<T>> store, Func<IEnumerable<T>, IEnumerable<T>> select) : base(store.GetValue)
		{
			_store = store;
			_select = @select;
		}

		public IEnumerator<T> GetEnumerator() => _store.Select(x => x.Value)
		                                               .Select(_select)
		                                               .Concat()
		                                               .GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}