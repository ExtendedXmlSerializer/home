using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Linq;

namespace ExtendedXmlSerializer
{

	/// <summary>
	/// Extension methods used for service location and registration.
	/// </summary>
	public static class ExtensionMethodsForLocation
	{
		/// <summary>
		/// Convenience method to inspect the provided type's dependencies and register them all.
		/// </summary>
		/// <param name="this">The repository to configure.</param>
		/// <typeparam name="T">The type to inspect.</typeparam>
		/// <returns>The configured repository.</returns>
		public static IServiceRepository RegisterWithDependencies<T>(this IServiceRepository @this)
			=> @this.Register<T>().RegisterDependencies(typeof(T));

		/// <summary>
		/// Convenience method to inspect the provided implementation type's dependencies and register them all.
		/// </summary>
		/// <typeparam name="TFrom">The base type.</typeparam>
		/// <typeparam name="TTo">The implementation type.</typeparam>
		/// <param name="this">The repository to configure.</param>
		/// <returns>The configured repository.</returns>
		public static IServiceRepository RegisterWithDependencies<TFrom, TTo>(this IServiceRepository @this)
			where TTo : TFrom
			=> @this.Register<TFrom, TTo>().RegisterDependencies(typeof(TTo));

		static IServiceRepository RegisterDependencies(this IServiceRepository @this, Type type)
			=> Constructors.Default.Get(type)
			               .SelectMany(x => x.GetParameters()
			                                 .Select(y => y.ParameterType))
			               .Where(x => x.IsClass && !@this.AvailableServices.Contains(x))
			               .Aggregate(@this, (repository, t) => repository.Register(t)
			                                                              .RegisterDependencies(t));
	}
}
