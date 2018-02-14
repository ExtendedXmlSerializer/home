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
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core.Collections
{
	public class GroupContainer<T> : DelegatedSource<GroupName, ICollection<T>>, IGroupContainer<T>
	{
		readonly IOrderedDictionary<GroupName, ICollection<T>> _store;

		public GroupContainer(IEnumerable<IGroup<T>> groups)
			: this(groups, GroupPairs<T>.Default) {}

		public GroupContainer(IEnumerable<IGroup<T>> groups, IGroupPairing<T> pairing)
			: this(new OrderedDictionary<GroupName, ICollection<T>>(groups.Select(pairing.Get))) {}

		public GroupContainer(IOrderedDictionary<GroupName, ICollection<T>> store) : base(store.GetValue) => _store = store;


		public IEnumerator<T> GetEnumerator() => _store.SelectMany(x => x.Value)
		                                               .GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}