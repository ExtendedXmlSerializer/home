using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core
{
	sealed class SortComparer<T> : IComparer<T> where T : class
	{
		public static SortComparer<T> Default { get; } = new SortComparer<T>();

		SortComparer() {}

		public int Compare(T x, T y) => GetSort(x)
			.CompareTo(GetSort(y));

		static int GetSort(T parameter) => (parameter as ISortAware)?.Sort ?? -1;
	}
}