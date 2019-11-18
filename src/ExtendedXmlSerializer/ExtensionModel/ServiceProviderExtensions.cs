using ExtendedXmlSerializer.Core;
using System;
using System.Collections.Generic;
// ReSharper disable TooManyArguments

namespace ExtendedXmlSerializer.ExtensionModel
{
	static class ServiceProviderExtensions
	{
		public static TService GetInstance<TService>(this IServiceProvider factory)
			=> (TService)factory.GetInstance(typeof(TService));

		public static TService GetInstance<TService>(this IServiceProvider factory, string serviceName)
			=> (TService)factory.GetInstance(typeof(TService), serviceName);

		public static TService GetInstance<T, TService>(this IServiceProvider factory, T value)
			=> (TService)factory.GetInstance(typeof(TService), new object[] {value});

		public static TService GetInstance<T, TService>(this IServiceProvider factory, T value, string serviceName)
			=> (TService)factory.GetInstance(typeof(TService), serviceName, new object[] {value});

		public static TService GetInstance<T1, T2, TService>(this IServiceProvider factory, T1 arg1, T2 arg2)
			=> (TService)factory.GetInstance(typeof(TService), new object[] {arg1, arg2});

		public static TService GetInstance<T1, T2, TService>(this IServiceProvider factory, T1 arg1, T2 arg2,
		                                                     string serviceName)
			=> (TService)factory.GetInstance(typeof(TService), serviceName, new object[] {arg1, arg2});

		public static TService GetInstance<T1, T2, T3, TService>(this IServiceProvider factory, T1 arg1, T2 arg2,
		                                                         T3 arg3)
			=> (TService)factory.GetInstance(typeof(TService), new object[] {arg1, arg2, arg3});

		public static TService GetInstance<T1, T2, T3, TService>(this IServiceProvider factory, T1 arg1, T2 arg2,
		                                                         T3 arg3,
		                                                         string serviceName)
			=> (TService)factory.GetInstance(typeof(TService), serviceName, new object[] {arg1, arg2, arg3});

		public static TService GetInstance<T1, T2, T3, T4, TService>(this IServiceProvider factory, T1 arg1, T2 arg2,
		                                                             T3 arg3,
		                                                             T4 arg4)
			=> (TService)factory.GetInstance(typeof(TService), new object[] {arg1, arg2, arg3, arg4});

		public static TService GetInstance<T1, T2, T3, T4, TService>(this IServiceProvider factory, T1 arg1, T2 arg2,
		                                                             T3 arg3,
		                                                             T4 arg4, string serviceName)
			=> (TService)factory.GetInstance(typeof(TService), serviceName, new object[] {arg1, arg2, arg3, arg4});

		public static TService TryGetInstance<TService>(this IServiceProvider factory)
			=> (TService)factory.TryGetInstance(typeof(TService));

		public static TService TryGetInstance<TService>(this IServiceProvider factory, string serviceName)
			=> (TService)factory.TryGetInstance(typeof(TService), serviceName);

		public static IEnumerable<TService> GetAllInstances<TService>(this IServiceProvider factory)
			=> factory.GetInstance<IEnumerable<TService>>();

		public static TService Create<TService>(this IServiceProvider factory) where TService : class
			=> (TService)factory.Create(typeof(TService));

		public static T Get<T>(this System.IServiceProvider @this, IServiceProvider _) => @this.Get<T>();

		public static T Get<T>(this System.IServiceProvider @this, Type type) => @this.GetService(type)
		                                                                              .AsValid<T>();
	}
}