using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// Provides a mechanism to dynamically access a generic singleton by away of typed parameter.
	/// </summary>
	/// <typeparam name="T">The singleton type.</typeparam>
	public interface IGeneric<out T> : IParameterizedSource<ImmutableArray<TypeInfo>, Func<T>> {}

	/// <summary>
	/// Provides a mechanism to dynamically create generic objects by way of typed parameters.
	/// </summary>
	/// <typeparam name="T1">The first parameter type.</typeparam>
	/// <typeparam name="T">The instantiated generic type instance.</typeparam>
	public interface IGeneric<in T1, out T> : IParameterizedSource<ImmutableArray<TypeInfo>, Func<T1, T>> {}

	/// <summary>
	/// Provides a mechanism to dynamically create generic objects by way of typed parameters.
	/// </summary>
	/// <typeparam name="T1">The first parameter type.</typeparam>
	/// <typeparam name="T2">The second parameter type.</typeparam>
	/// <typeparam name="T">The instantiated generic type instance.</typeparam>
	public interface IGeneric<in T1, in T2, out T> : IParameterizedSource<ImmutableArray<TypeInfo>, Func<T1, T2, T>> {}

	/// <summary>
	/// Provides a mechanism to dynamically create generic objects by way of typed parameters.
	/// </summary>
	/// <typeparam name="T1">The first parameter type.</typeparam>
	/// <typeparam name="T2">The second parameter type.</typeparam>
	/// <typeparam name="T3">The third parameter type.</typeparam>
	/// <typeparam name="T">The instantiated generic type instance.</typeparam>
	public interface IGeneric<in T1, in T2, in T3, out T>
		: IParameterizedSource<ImmutableArray<TypeInfo>, Func<T1, T2, T3, T>> {}

	/// <summary>
	/// Provides a mechanism to dynamically create generic objects by way of typed parameters.
	/// </summary>
	/// <typeparam name="T1">The first parameter type.</typeparam>
	/// <typeparam name="T2">The second parameter type.</typeparam>
	/// <typeparam name="T3">The third parameter type.</typeparam>
	/// <typeparam name="T4">The fourth parameter type.</typeparam>
	/// <typeparam name="T">The instantiated generic type instance.</typeparam>
	public interface IGeneric<in T1, in T2, in T3, in T4, out T>
		: IParameterizedSource<ImmutableArray<TypeInfo>, Func<T1, T2, T3, T4, T>> {}
}