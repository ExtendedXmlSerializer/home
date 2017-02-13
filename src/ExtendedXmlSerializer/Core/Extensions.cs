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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Core
{
	public static class Extensions
	{
		readonly static char[] Delimiters = {','};

		public static T[] Fixed<T>(this IEnumerable<T> @this) => @this as T[] ?? @this.ToArray();

		public static ImmutableArray<string> ToStringArray(this string target) => ToStringArray(target, Delimiters);

		public static ImmutableArray<string> ToStringArray(this string target, params char[] delimiters) =>
			target.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToImmutableArray();

		public static TypeInfo AccountForNullable(this TypeInfo @this)
			=> Nullable.GetUnderlyingType(@this.AsType())?.GetTypeInfo() ?? @this;

		public static IParameterizedSource<TParameter, TResult> Cache<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this)
			=> CachingAlteration<TParameter, TResult>.Default.Get(@this);

		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key) where TValue : class
		{
			TValue result;
			return target.TryGetValue(key, out result) ? result : default(TValue);
		}

		public static IEnumerable<TValue> Get<TKey, TValue>(this ILookup<TKey, TValue> target, TKey key) => target[key];

		public static TValue? GetStructure<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key)
			where TValue : struct
		{
			TValue result;
			return target.TryGetValue(key, out result) ? result : (TValue?) null;
		}

		public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, params T[] items) => @this.Concat(items);

		public static string NullIfEmpty(this string target) => string.IsNullOrEmpty(target) ? null : target;

		public static T Self<T>(this T @this) => @this;
		public static TResult Accept<TParameter, TResult>(this TResult @this, TParameter _) => @this;

		public static IEnumerable<T> Yield<T>(this T @this)
		{
			yield return @this;
		}

		public static IEnumerable<TypeInfo> YieldMetadata(this IEnumerable<Type> @this,
		                                                  Func<TypeInfo, bool> specification = null)
		{
			var select = @this.Select(x => x.GetTypeInfo());
			var result = specification != null ? select.Where(specification) : select;
			return result;
		}

		public static ImmutableArray<TypeInfo> ToMetadata(this IEnumerable<Type> @this,
		                                                  Func<TypeInfo, bool> specification = null)
			=> @this.YieldMetadata(specification).ToImmutableArray();


		public static ISpecification<T> Inverse<T>(this ISpecification<T> @this) => new InverseSpecification<T>(@this);

		public static ISpecification<object> Adapt<T>(this ISpecification<T> @this)
			=> new SpecificationAdapter<T>(@this);

		public static T To<T>(this object @this) => @this is T ? (T) @this : default(T);

		public static T Get<T>(this IServiceProvider @this)
			=> @this is T ? (T) @this : @this.GetService(typeof(T)).To<T>();

		public static T AsValid<T>(this object @this, string message = null)
		{
			if (@this is T)
			{
				return (T) @this;
			}

			throw new InvalidOperationException(message ??
			                                    $"'{@this.GetType().FullName}' is not of type {typeof(T).FullName}.");
		}
	}
}