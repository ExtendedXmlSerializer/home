using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public interface IGeneric<out T> : IParameterizedSource<ImmutableArray<TypeInfo>, Func<T>> {}

	public interface IGeneric<in T1, out T> : IParameterizedSource<ImmutableArray<TypeInfo>, Func<T1, T>> {}

	public interface IGeneric<in T1, in T2, out T> : IParameterizedSource<ImmutableArray<TypeInfo>, Func<T1, T2, T>> {}

	public interface IGeneric<in T1, in T2, in T3, out T>
		: IParameterizedSource<ImmutableArray<TypeInfo>, Func<T1, T2, T3, T>> {}

	public interface IGeneric<in T1, in T2, in T3, in T4, out T>
		: IParameterizedSource<ImmutableArray<TypeInfo>, Func<T1, T2, T3, T4, T>> {}
}