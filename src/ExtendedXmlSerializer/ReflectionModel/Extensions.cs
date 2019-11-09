using System;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	static class Extensions
	{
		public static T Get<T>(this IParameterizedSource<ImmutableArray<TypeInfo>, T> @this,
		                       params TypeInfo[] parameters)
			=> @this.Get(parameters.ToImmutableArray());

		public static T New<T>(this IActivators @this) => New<T>(@this, typeof(T));

		public static T New<T>(this IActivators @this, Type type) => (T)@this.Get(type)
		                                                                     .Get();

		public static IEnumerator For(this IEnumeratorStore @this, object parameter)
			=> @this.Get(parameter.GetType()
			                      .GetTypeInfo())
			        ?.Get((IEnumerable)parameter);

		public static bool IsSatisfiedBy(this ISpecification<TypeInfo> @this, object parameter)
			=> @this.IsSatisfiedBy(parameter.GetType()
			                                .GetTypeInfo());

		public static bool IsSatisfiedBy(this ISpecification<TypeInfo> @this, Type parameter)
			=> @this.IsSatisfiedBy(parameter.GetTypeInfo());

		public static bool IsSatisfiedBy(this ISpecification<Type> @this, TypeInfo parameter)
			=> @this.IsSatisfiedBy(parameter.AsType());
	}
}