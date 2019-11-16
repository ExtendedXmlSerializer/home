using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// General-purpose extension methods.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>Convenience method used for fluent-type methods.</summary>
		/// <typeparam name="T">The type of the calling instance.</typeparam>
		/// <typeparam name="TOut">The result type.</typeparam>
		/// <param name="_">Not used.</param>
		/// <param name="result">The result.</param>
		/// <returns>TOut.</returns>
		public static TOut Return<T, TOut>(this T _, TOut result) => result;

		/// <summary>
		/// Convenience method for objects that implement several <see cref="ISource{T}"/> to get its type-based contents
		/// specifically.
		/// </summary>
		/// <param name="this">The implementing source.</param>
		/// <returns>TypeInfo.</returns>
		public static TypeInfo GetType(this ISource<TypeInfo> @this) => @this.Get();

		/// <summary>
		/// Convenience method for objects that implement several <see cref="ISource{T}"/> to get its member-based contents
		/// specifically.
		/// </summary>
		/// <param name="this">The implementing source.</param>
		/// <returns>MemberInfo.</returns>
		public static MemberInfo GetMember(this ISource<MemberInfo> @this) => @this.Get();

		/// <summary>
		/// Convenience method to invoke a method and return the parameter.  This is useful for fluent-based configuration
		/// method calls.
		/// </summary>
		/// <typeparam name="T">The parameter type.</typeparam>
		/// <param name="this">The delegate to call.</param>
		/// <param name="parameter">The parameter to pass to the delegate and return.</param>
		/// <returns>The delegate.</returns>
		public static T Apply<T>(this Action<T> @this, T parameter)
		{
			@this(parameter);
			return parameter;
		}

		/// <summary>
		/// Convenience method to pass values to an assignable command, and return the command.  This is useful for
		/// fluent-based configuration method calls.
		/// </summary>
		/// <typeparam name="TKey">The key type.</typeparam>
		/// <typeparam name="TValue">The value type.</typeparam>
		/// <param name="this">The assignable to command to invoke.</param>
		/// <param name="key">The key to pass in.</param>
		/// <param name="value">The value to pass in.</param>
		/// <returns>The assignable command.</returns>
		public static IAssignable<TKey, TValue> Apply<TKey, TValue>(this IAssignable<TKey, TValue> @this, TKey key,
		                                                            TValue value)
		{
			@this.Assign(key, value);
			return @this;
		}

		/// <summary>
		/// Convenience method to add an element to a collection, and return the element.  This is useful for
		/// fluent-based configuration method calls.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="this">The collection to add an element to.</param>
		/// <param name="parameter">The element to add.</param>
		/// <returns>The element.</returns>
		public static T Apply<T>(this ICollection<T> @this, T parameter)
		{
			@this.Add(parameter);
			return parameter;
		}
	}
}
