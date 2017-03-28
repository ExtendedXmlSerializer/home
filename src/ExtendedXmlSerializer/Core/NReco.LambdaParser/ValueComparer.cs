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
using System.Globalization;
using System.Reflection;

// ReSharper disable All

namespace ExtendedXmlSerializer.Core.NReco.LambdaParser
{
	/// <summary>
	/// Generic "by value" comparer that uses ConvertManager for types harmonization
	/// </summary>
	internal class ValueComparer : IComparer, IComparer<object>
	{
		static ValueComparer _Instance = new ValueComparer();

		public static ValueComparer Instance
		{
			get { return _Instance; }
		}

		private bool IsAssignableFrom(Type a, Type b)
		{
#if NET40
			return a.IsAssignableFrom(b);
			#else
			return a.GetTypeInfo().IsAssignableFrom(b.GetTypeInfo());
#endif
		}

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
				var aList = (IList) a;
				var bList = (IList) b;
				if (aList.Count < bList.Count)
					return -1;
				if (aList.Count > bList.Count)
					return +1;
				for (var i = 0; i < aList.Count; i++)
				{
					var r = Compare(aList[i], bList[i]);
					if (r != 0)
						return r;
				}
				// lists are equal
				return 0;
			}
			// test for quick compare if a type is assignable from b
			if (a is IComparable)
			{
				var aComp = (IComparable) a;
				// quick compare if types are fully compatible
				if (IsAssignableFrom(a.GetType(), b.GetType()))
					return aComp.CompareTo(b);
			}
			if (b is IComparable)
			{
				var bComp = (IComparable) b;
				// quick compare if types are fully compatible
				if (IsAssignableFrom(b.GetType(), a.GetType()))
					return -bComp.CompareTo(a);
			}

			// try to convert b to a and then compare
			if (a is IComparable)
			{
				var aComp = (IComparable) a;
				var bConverted = Convert.ChangeType(b, a.GetType(), CultureInfo.InvariantCulture);
				return aComp.CompareTo(bConverted);
			}
			// try to convert a to b and then compare
			if (b is IComparable)
			{
				var bComp = (IComparable) b;
				var aConverted = Convert.ChangeType(a, b.GetType(), CultureInfo.InvariantCulture);
				return -bComp.CompareTo(aConverted);
			}

			throw new InvalidCastException(String.Format("Cannot compare {0} and {1}", a.GetType(), b.GetType()));
		}
	}
}