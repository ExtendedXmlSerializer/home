using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
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
		/// Convenience method to invoke a method and return the parameter.  This is useful for fluent-based configuration
		/// method calls.
		/// </summary>
		/// <typeparam name="T">The parameter type.</typeparam>
		/// <param name="this">The delegate to call.</param>
		/// <param name="parameter">The parameter to pass to the delegate and return.</param>
		/// <returns>The provided parameter.</returns>
		public static T Apply<T>(this Action<T> @this, T parameter)
		{
			@this(parameter);
			return parameter;
		}

		/// <summary>
		/// Convenience method to invoke a command and return the parameter.  This is useful for fluent-based configuration
		/// method calls.
		/// </summary>
		/// <typeparam name="T">The parameter type.</typeparam>
		/// <param name="this">The command to call.</param>
		/// <param name="parameter">The parameter to pass to the command and return.</param>
		/// <returns>The provided parameter.</returns>
		public static T Apply<T>(this ICommand<T> @this, T parameter)
		{
			@this.Execute(parameter);
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

		/// <summary>
		/// Convenience method that queries an expression to resolve a property or field expression.
		/// </summary>
		/// <param name="expression">The expression to query.</param>
		/// <param name="type">The member containing type.</param>
		/// <param name="name">The member name.</param>
		/// <returns>The resolved member expression.</returns>
		public static MemberExpression PropertyOrField(this UnaryExpression expression, Type type, string name)
		{
			var typeInfo = type.GetTypeInfo();
			var property = typeInfo.GetDeclaredProperty(name) ??
			               typeInfo.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
			if (property != null)
			{
				return Expression.Property(expression, property);
			}

			var field = type.GetField(name) ?? type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
			if (field != null)
			{
				return Expression.Field(expression, field);
			}

			throw new InvalidOperationException($"Could not find the member '{name}' on type '{type}'.");
		}

		/// <summary>
		/// Gets the only element if it exists, otherwise returns the type's default.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="this">The set to query.</param>
		/// <returns>The only element, if it exists.</returns>
		public static T Only<T>(this ImmutableArray<T> @this) => @this.Length == 1 ? @this[0] : default;

		/// <summary>
		/// Gets the only element if it exists, otherwise returns the type's default.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="this">The set to query.</param>
		/// <returns>The only element, if it exists.</returns>
		public static T Only<T>(this IEnumerable<T> @this)
		{
			var items  = @this.ToArray();
			var result = items.Length == 1 ? items[0] : default;
			return result;
		}

		/// <summary>
		/// Convenience method to case object to requested type.  Easier for fluent expressions.
		/// </summary>
		/// <param name="this">The object to cast.</param>
		/// <typeparam name="T">The requested type.</typeparam>
		/// <returns>An instance of the request type if it can be cast.  Otherwise, the type's default is returned.</returns>
		public static T To<T>(this object @this) => @this is T o ? o : default;

		/// <summary>
		/// Convenience method to cast instance to the requested type.  If it cannot, an <see cref="InvalidOperationException"/> is thrown.
		/// </summary>
		/// <typeparam name="T">The requested type</typeparam>
		/// <param name="this">The instance to cast.</param>
		/// <param name="message">The message to display if an exception is thrown.</param>
		/// <returns>The casted, validated instance.</returns>
		public static T AsValid<T>(this object @this, string message = null)
			=> @this != null
				   ? @this is T o
					     ? o
					     : throw new InvalidOperationException(message ??
					                                           $"'{@this.GetType().FullName}' is not of type {typeof(T).FullName}.")
				   : default;
	}
}