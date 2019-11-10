using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public static class FrameworkExtensions
	{
		public static IServiceRepository RegisterWithDependencies<T>(this IServiceRepository @this)
			=> @this.Register<T>()
			        .RegisterDependencies(typeof(T));

		public static IServiceRepository RegisterWithDependencies<TFrom, TTo>(this IServiceRepository @this)
			where TTo : TFrom
			=> @this.Register<TFrom, TTo>()
			        .RegisterDependencies(typeof(TTo));

		static IServiceRepository RegisterDependencies(this IServiceRepository @this, Type type)
			=> Constructors.Default.Get(type)
			               .SelectMany(x => x.GetParameters()
			                                 .Select(y => y.ParameterType))
			               .Where(x => x.IsClass && !@this.AvailableServices.Contains(x))
			               .Aggregate(@this, (repository, t) => repository.Register(t)
			                                                              .RegisterDependencies(t));
	}
}