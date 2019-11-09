// ReSharper disable All

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ExtendedXmlSerializer.Core.NReco.LambdaParser
{
	/// <summary>
	/// Generic "by value" comparer that uses ConvertManager for types harmonization
	/// </summary>
	sealed class ValueComparer : IComparer, IComparer<object>
	{
		static ValueComparer _Instance = new ValueComparer();

		public static ValueComparer Instance => _Instance;

		public int Compare(object a, object b)
		{
			if (a == null && b == null)
				return 0;
			if (a == null && b != null)
				return -1;
			if (a != null && b == null)
				return 1;

			if ((a is IList) && (b is IList))
			{
				IList aList = (IList)a;
				IList bList = (IList)b;
				if (aList.Count < bList.Count)
					return -1;
				if (aList.Count > bList.Count)
					return +1;
				for (int i = 0; i < aList.Count; i++)
				{
					int r = Compare(aList[i], bList[i]);
					if (r != 0)
						return r;
				}

				// lists are equal
				return 0;
			}

			// test for quick compare if a type is assignable from b
			if (a is IComparable)
			{
				var aComp = (IComparable)a;
				// quick compare if types are fully compatible
				if (a.GetType()
				     .IsAssignableFrom(b.GetType()))
					return aComp.CompareTo(b);
			}

			if (b is IComparable)
			{
				var bComp = (IComparable)b;
				// quick compare if types are fully compatible
				if (b.GetType()
				     .GetTypeInfo()
				     .IsAssignableFrom(a.GetType()))
					return -bComp.CompareTo(a);
			}

			// try to convert b to a and then compare
			if (a is IComparable)
			{
				var aComp      = (IComparable)a;
				var bConverted = Convert.ChangeType(b, a.GetType(), CultureInfo.InvariantCulture);
				return aComp.CompareTo(bConverted);
			}

			// try to convert a to b and then compare
			if (b is IComparable)
			{
				var bComp      = (IComparable)b;
				var aConverted = Convert.ChangeType(a, b.GetType(), CultureInfo.InvariantCulture);
				return -bComp.CompareTo(aConverted);
			}

			throw new InvalidCastException(String.Format("Cannot compare {0} and {1}", a.GetType(), b.GetType()));
		}
	}
}