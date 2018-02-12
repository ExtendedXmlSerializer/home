using System;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Collections {
	/// <summary>
	/// ATTRIBUTION: https://github.com/mattmc3/dotmore
	/// </summary>
	public class Comparer2<T> : Comparer<T>
	{
		private readonly Comparison<T> _compareFunction;

		public Comparer2(Comparison<T> comparison)
		{
			if (comparison == null) throw new ArgumentNullException("comparison");
			_compareFunction = comparison;
		}

		public override int Compare(T arg1, T arg2) => _compareFunction(arg1, arg2);
	}
}