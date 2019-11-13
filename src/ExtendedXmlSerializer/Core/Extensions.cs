using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
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
			=>
				new DelegatedSource<TParameter, TResult>(
				                                         ReferenceCachingAlteration<TParameter, TResult>
					                                         .Default.Get(@this.ToSelectionDelegate()));

		public static T With<T>(this T @this, Action<T> action)
		{
			action(@this);
			return @this;
		}

		public static T Alter<T>(this T @this, Func<T, T> action) => @this != null ? action(@this) : default;

		public static string Quoted(this string @this) => QuotedAlteration.Default.Get(@this);

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
			this IEnumerable<KeyValuePair<TKey, TValue>> @this,
			IEqualityComparer<TKey> comparer = null)
			=> @this.ToDictionary(x => x.Key, x => x.Value, comparer);

		public static T Only<T>(this ImmutableArray<T> @this) => @this.Length == 1 ? @this[0] : default;

		public static T Only<T>(this IEnumerable<T> @this)
		{
			var items  = @this.ToArray();
			var result = items.Length == 1 ? items[0] : default;
			return result;
		}

		public static ITypedSortOrder Sort<T>(this ITypedSortOrder @this, int sort)
		{
			@this.Assign(Support<T>.Metadata, sort);
			return @this;
		}

		public static MemberInfo GetMemberInfo(this Expression expression)
		{
			var lambda = (LambdaExpression)expression;
			var member =
				(lambda.Body.AsTo<UnaryExpression, Expression>(unaryExpression => unaryExpression.Operand) ??
				 lambda.Body)
				.To<MemberExpression>()
				.Member;

			return member;
		}

		static MemberInfo Member(this Expression expression)
		{
			var lambda = (LambdaExpression)expression;
			var member =
				(lambda.Body.AsTo<UnaryExpression, Expression>(unaryExpression => unaryExpression.Operand) ??
				 lambda.Body)
				.To<MemberExpression>()
				.Member;

			return member;
		}

		public static MemberInfo GetMemberInfo<T, TMember>(this Expression<Func<T, TMember>> expression)
			=> Support<T>.Metadata.GetMember(expression.Member()
			                                      .Name)
			             .Single();

		public static TResult AsTo<TSource, TResult>(this object target, Func<TSource, TResult> transform,
		                                             Func<TResult> resolve = null)
		{
			var @default = resolve ?? (() => default);
			var result   = target is TSource ? transform((TSource)target) : @default();
			return result;
		}

		public static IEnumerable<T> TypeZip<T>(this IEnumerable<T> @this, IEnumerable<T> other)
		{
			var items = other.ToDictionary(x => x.GetType(), x => x);
			foreach (var item in @this)
			{
				T   found;
				var key = item.GetType();
				yield return items.TryGetValue(key, out found) && items.Remove(key) ? found : item;
			}

			foreach (var item in items.Values)
			{
				yield return item;
			}
		}

		public static T[] Fixed<T>(this IEnumerable<T> @this) => @this as T[] ?? @this.ToArray();

		public static KeyedByTypeCollection<T> KeyedByType<T>(this IEnumerable<T> @this) =>
			@this as KeyedByTypeCollection<T> ?? new KeyedByTypeCollection<T>(@this);

		public static ICollection<T> AddOrReplace<T, TItem>(this ICollection<T> @this, TItem item)
		{
			var source = @this.KeyedByType()
			                  .AddOrReplace(item);
			@this.SynchronizeFrom(source);
			return @this;
		}

		public static bool Removing<T, TItem>(this ICollection<T> @this, TItem item)
		{
			var source = @this.KeyedByType();
			var result = source.Remove(item);
			@this.SynchronizeFrom(source);
			return result;
		}

		public static ICollection<T> SynchronizeFrom<T>(this ICollection<T> @this, IEnumerable<T> source)
		{
			if (!ReferenceEquals(@this, source))
			{
				@this.Clear();
				foreach (var item in source)
				{
					@this.Add(item);
				}
			}

			return @this;
		}

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
			=> target.TryGetValue(key, out var result) ? result : default;

		public static IEnumerable<TValue> Get<TKey, TValue>(this ILookup<TKey, TValue> target, TKey key) => target[key];

		public static TValue? GetStructure<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key)
			where TValue : struct
			=> AssignedSpecification<TKey>.Default.IsSatisfiedBy(key) && target.TryGetValue(key, out var result)
				   ? result
				   : (TValue?)null;

		public static IEnumerable<T> Appending<T>(this IEnumerable<T> @this, params T[] items) => @this.Concat(items);

		public static string NullIfEmpty(this string target) => string.IsNullOrEmpty(target) ? null : target;

		public static T Self<T>(this T @this) => @this;

		public static TResult Accept<TParameter, TResult>(this TResult @this, TParameter _) => @this;

		public static IEnumerable<T> Yield<T>(this T @this)
		{
			yield return @this;
		}

		public static IEnumerable<TypeInfo> YieldMetadata(this IEnumerable<Type> @this)
			=> @this.Select(x => x.GetTypeInfo());

		public static ImmutableArray<TypeInfo> ToMetadata(this IEnumerable<Type> @this)
			=> @this.YieldMetadata()
			        .ToImmutableArray();

		public static IEnumerable<Type> YieldTypes(this IEnumerable<TypeInfo> @this)
			=> @this.Select(x => x.GetTypeInfo());

		public static ImmutableArray<Type> ToTypes(this IEnumerable<TypeInfo> @this)
			=> @this.YieldTypes()
			        .ToImmutableArray();

		public static T To<T>(this object @this) => @this is T ? (T)@this : default;

		public static T Get<T>(this IServiceProvider @this)
			=> @this is T
				   ? (T)@this
				   : @this.GetService(typeof(T))
				          .To<T>();

		public static T GetValid<T>(this IServiceProvider @this)
			=> @this is T
				   ? (T)@this
				   : @this.GetService(typeof(T))
				          .AsValid<T>($"Could not located service '{typeof(T)}'");

		public static void ForEach<TIn, TOut>(this IEnumerable<TIn> @this, Func<TIn, TOut> select)
		{
			foreach (var @in in @this)
			{
				select(@in);
			}
		}

		public static T AsValid<T>(this object @this, string message = null)
		{
			if (@this != null)
			{
				if (@this is T)
				{
					return (T)@this;
				}

				throw new InvalidOperationException(message ??
				                                    $"'{@this.GetType().FullName}' is not of type {typeof(T).FullName}.");
			}

			return default;
		}
	}
}