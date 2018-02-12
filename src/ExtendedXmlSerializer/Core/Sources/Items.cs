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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Sources
{
	public interface IEnumerableAlteration<T> : IAlteration<IEnumerable<T>> {}

	class OrderByAlteration<T, TMember> : IEnumerableAlteration<T>
	{
		readonly Func<T, TMember> _select;

		public OrderByAlteration(Func<T, TMember> select) => _select = @select;

		public IEnumerable<T> Get(IEnumerable<T> parameter) => parameter is IOrderedEnumerable<T> ordered ? ordered.ThenBy(_select) : parameter.OrderBy(_select);
	}

	public class EnumerableAlterations<T> : IEnumerableAlteration<T>
	{
		readonly ImmutableArray<IAlteration<IEnumerable<T>>> _alterations;

		public EnumerableAlterations(params IAlteration<IEnumerable<T>>[] alterations) : this(alterations.ToImmutableArray()) {}

		public EnumerableAlterations(ImmutableArray<IAlteration<IEnumerable<T>>> alterations) => _alterations = alterations;

		public IEnumerable<T> Get(IEnumerable<T> parameter) => _alterations.Aggregate(parameter, (enumerable, alteration) => alteration.Get(enumerable));
	}


	public class Items<T> : ItemsBase<T>
	{
		readonly ImmutableArray<T> _items;

		public Items(params T[] items) : this(items.AsEnumerable()) {}

		public Items(IEnumerable<T> items) : this(items.ToImmutableArray()) {}

		public Items(ImmutableArray<T> items) => _items = items;

		public sealed override IEnumerator<T> GetEnumerator()
		{
			var length = _items.Length;
			for (var i = 0; i < length; i++)
			{
				yield return _items[i];
			}
		}
	}
}