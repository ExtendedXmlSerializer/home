using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Collections
{
	/// <summary>
	/// ATTRIBUTION: https://github.com/mattmc3/dotmore
	/// </summary>
	public static class EnumerableExtensions
	{
		public static T FirstOfType<T>(this IEnumerable @this) => @this.OfType<T>().FirstOrDefault();

		public static IEnumerable<T> AsEnumerable<T>(this ImmutableArray<T> @this)
		{
			var length = @this.Length;
			for (var i = 0; i < length; i++)
			{
				yield return @this[i];
			}
		}

		public static IEnumerable<T> Assigned<T>(this IEnumerable<T> @this) => @this.Where(Delegate<T>.Default.Get());

		sealed class Delegate<T> : FixedInstanceSource<Func<T, bool>>
		{
			public static Delegate<T> Default { get; } = new Delegate<T>();
			Delegate() : this(AssignedSpecification<T>.Default.IsSatisfiedBy) {}

			public Delegate(Func<T, bool> instance) : base(instance) {}
		}

		public static T Coalesce<T>(this IEnumerable<T> e)
		{
			if (e == null) return default(T);
			return e.FirstOrDefault(x => x != null);
		}

		public static IEnumerable<T> Prepending<T>(this IEnumerable<T> e, T firstElement) => (new[] { firstElement }).Concat(e);

		public static IEnumerable<T> Append<T>(this IEnumerable<T> e, T lastElement) => e.Concat(new[] { lastElement });

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> e)
		{
			var list = e.ToList();
			var random = new Random();

			for (int i = list.Count - 1; i >= 0; i--)
			{
				int r = random.Next(i + 1);
				yield return list[r];
				list[r] = list[i];
			}
		}

		/// <summary>
		/// Groups items together in an enumerable while a condition is true
		/// </summary>
		/// <remarks>http://stackoverflow.com/questions/16306770/how-can-i-efficiently-do-master-detail-grouping-in-linq-from-a-flat-enumerable-w/16307599?noredirect=1#comment23349239_16307599</remarks>
		public static IEnumerable<IEnumerable<T>> GroupWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			using (var iterator = source.GetEnumerator())
			{
				if (!iterator.MoveNext())
				{
					yield break;
				}

				List<T> list = new List<T>() { iterator.Current };

				while (iterator.MoveNext())
				{
					if (predicate(iterator.Current))
					{
						list.Add(iterator.Current);
					}
					else
					{
						yield return list;
						list = new List<T>() { iterator.Current };
					}
				}
				yield return list;
			}
		}

		public static OrderedDictionary<TKey, TSource> ToOrderedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
			=> GetOrderedDictionaryImpl(source, keySelector, x => x, null);

		public static OrderedDictionary<TKey, TElement> ToOrderedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) => GetOrderedDictionaryImpl(source, keySelector, elementSelector, null);

		public static OrderedDictionary<TKey, TSource> ToOrderedDictionary<TSource, TKey>(
			this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
			=> GetOrderedDictionaryImpl(source, keySelector, x => x, comparer);

		public static OrderedDictionary<TKey, TElement> ToOrderedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) => GetOrderedDictionaryImpl(source, keySelector, elementSelector, comparer);

		public static string ToDelimitedString(this IEnumerable source, string delimiter = ",", string quote = "\"", Func<object, string> convertToString = null)
		{
			if (convertToString == null)
			{
				convertToString = x => x?.ToString() ?? string.Empty;
			}

			var data = (
				           from object a in source
				           let s = convertToString(a)
				           select s.Contains(delimiter) || s.Contains("\r") || s.Contains("\n") || s.Contains(quote) ? String.Format("{0}{1}{0}", quote, s) : s
			           );
			return String.Join(",", data);
		}

		private static OrderedDictionary<TKey, TElement> GetOrderedDictionaryImpl<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			var result = comparer == null ? new OrderedDictionary<TKey, TElement>() : new OrderedDictionary<TKey, TElement>(comparer);

			foreach (TSource sourceItem in source)
			{
				result.Add(keySelector(sourceItem), elementSelector(sourceItem));
			}

			return result;
		}

	}
}