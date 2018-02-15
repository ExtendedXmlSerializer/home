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

using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.Core
{
	static class Extensions
	{
		public static IParameterizedSource<TParameter, TResult>
			ReferenceCache<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this)
			where TParameter : class where TResult : class
			=> new DelegatedSource<TParameter, TResult>(ReferenceCachingAlteration<TParameter, TResult>
			                                            .Default.Get(@this.ToDelegate()));

		public static T With<T>(this T @this, Action<T> action)
		{
			action(@this);
			return @this;
		}

		public static T Alter<T>(this T @this, Func<T, T> action) => @this != null ? action(@this) : default(T);


		public static string Quoted(this string @this) => QuotedAlteration.Default.Get(@this);

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> @this,
		                                                                  IEqualityComparer<TKey> comparer = null)
			=> @this.ToDictionary(x => x.Key, x => x.Value, comparer);

		public static T Only<T>(this ImmutableArray<T> @this) => @this.Length == 1 ? @this[0] : default(T);

		public static T Only<T>(this IEnumerable<T> @this)
		{
			var items = @this.ToArray();
			var result = items.Length == 1 ? items[0] : default(T);
			return result;
		}

		public static ITypedSortOrder Sort<T>(this ITypedSortOrder @this, int sort)
		{
			@this.Assign(Support<T>.Key, sort);
			return @this;
		}

		public static MemberInfo GetMemberInfo(this Expression expression)
		{
			var lambda = (LambdaExpression) expression;
			var result =
				(lambda.Body.AsTo<UnaryExpression, Expression>(unaryExpression => unaryExpression.Operand) ?? lambda.Body)
				.To<MemberExpression>()
				.Member;
			return result;
		}

		public static TResult Return<T, TResult>(this T _, TResult result) => result;

		public static TResult AsTo<TSource, TResult>(this object target, Func<TSource, TResult> transform,
		                                             Func<TResult> resolve = null)
		{
			var @default = resolve ?? (() => default(TResult));
			var result = target is TSource ? transform((TSource) target) : @default();
			return result;
		}

		public static T[] Fixed<T>(this IEnumerable<T> @this) => @this as T[] ?? @this.ToArray();

		public static ICollection<T> Adding<T>(this ICollection<T> @this, T item)
		{
			@this.Add(item);
			return @this;
		}

		public static ICollection<T> AddingAll<T>(this ICollection<T> @this, params T[] items)
		{
			items.ForEach(@this.Add);
			return @this;
		}

		public static ICollection<T> Removing<T>(this ICollection<T> @this, T item)
		{
			@this.Remove(item);
			return @this;
		}

		/*public static ICollection<TItem> AddOrReplace<T, TItem>(this ICollection<TItem> @this, T item) where T : TItem
		{
			@this.RemoveAll(A<T>.Default);
			@this.Add(item);
			return @this;
		}

		public static ICollection<TItem> RemoveAll<T, TItem>(this ICollection<TItem> @this, A<T> _) where T : TItem
		{
			@this.OfType<T>()
			     .OfType<TItem>()
			     .ToArray()
			     .ForEach(@this.Remove);
			return @this;
		}*/

		public static TypeInfo AccountForNullable(this TypeInfo @this)
			=> AccountForNullableAlteration.Default.Get(@this);

		public static Func<TParameter, TResult>
			ReferenceCache<TParameter, TResult>(this Func<TParameter, TResult> @this)
			where TParameter : class where TResult : class
			=> ReferenceCachingAlteration<TParameter, TResult>.Default.Get(@this);

		public static Func<TParameter, TResult> Cache<TParameter, TResult>(
			this Func<TParameter, TResult> @this)
			=> CachingAlteration<TParameter, TResult>.Default.Get(@this);

		public static TValue Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> target, TKey key)
			=> target.TryGetValue(key, out var result) ? result : default(TValue);

		public static IEnumerable<TValue> Get<TKey, TValue>(this ILookup<TKey, TValue> target, TKey key) => target[key];

		public static TValue? GetStructure<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key)
			where TValue : struct => target.TryGetValue(key, out var result) ? result : (TValue?) null;

		public static IEnumerable<T> Appending<T>(this IEnumerable<T> @this, params T[] items) => @this.Concat(items);

		public static string NullIfEmpty(this string target) => string.IsNullOrEmpty(target) ? null : target;

		public static T Self<T>(this T @this) => @this;
		public static TResult Accept<TParameter, TResult>(this TResult @this, TParameter _) => @this;

		public static IEnumerable<T> Yield<T>(this T @this)
		{
			yield return @this;
		}

		public static IEnumerable<TypeInfo> YieldMetadata(this IEnumerable<Type> @this) => @this.Select(x => x.GetTypeInfo());

		public static ImmutableArray<TypeInfo> ToMetadata(this IEnumerable<Type> @this)
			=> @this.YieldMetadata()
			        .ToImmutableArray();

		public static IEnumerable<Type> YieldTypes(this IEnumerable<TypeInfo> @this) => @this.Select(x => x.GetTypeInfo());

		public static ImmutableArray<Type> ToTypes(this IEnumerable<TypeInfo> @this)
			=> @this.YieldTypes()
			        .ToImmutableArray();


		public static T To<T>(this object @this, string message = null)
		{
			if (@this is T result)
			{
				return result;
			}

			throw new InvalidOperationException(message ??
			                                    $"'{@this.GetType().FullName}' is not of type {typeof(T).FullName}.");
		}

		public static T Get<T>(this IServiceProvider @this)
		{
			if (@this is T instance)
			{
				return instance;
			}

			var service = @this.GetService(typeof(T));
			var result  = service != null ? service.To<T>() : default(T);
			return result;
		}

		public static T GetValid<T>(this IServiceProvider @this)
		{
			var message = $"Could not locate service '{typeof(T)}'";
			var service = @this.GetService(typeof(T)) ?? throw new InvalidOperationException(message);
			var result = service.To<T>(message);
			return result;
		}

		public static void ForEach<TIn, TOut>(this IEnumerable<TIn> @this, Func<TIn, TOut> select)
		{
			foreach (var @in in @this)
			{
				select(@in);
			}
		}
	}
}