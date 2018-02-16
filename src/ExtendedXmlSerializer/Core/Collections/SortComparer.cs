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
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Collections
{
	sealed class SortComparer<T> : IComparer<T>
	{
		public static SortComparer<T> Default { get; } = new SortComparer<T>();
		SortComparer() : this(SortCoercer<T>.Default.Get) {}

		readonly Func<T, int> _sort;

		public SortComparer(Func<T, int> sort) => _sort = sort;

		public int Compare(T x, T y) => _sort(x).CompareTo(_sort(y));
	}

	sealed class SortCoercer<T> : DecoratedSource<T, int>
	{
		public static SortCoercer<T> Default { get; } = new SortCoercer<T>();
		SortCoercer() : base(Assume<T>.Default(-1)
		                              .Unless(SortMetadata<T>.Default)
		                              .Unless(A<ISortAware>.Default)) {}
	}

	sealed class SortMetadata<T> : InstanceMetadataValue<SortAttribute, T, int>
	{
		public static SortMetadata<T> Default { get; } = new SortMetadata<T>();
		SortMetadata() {}
	}

	[AttributeUsage(AttributeTargets.Class)]
	sealed class SortAttribute : Attribute, ISource<int>
	{
		readonly int _sort;

		public SortAttribute(int sort) => _sort = sort;

		public int Get() => _sort;
	}
}