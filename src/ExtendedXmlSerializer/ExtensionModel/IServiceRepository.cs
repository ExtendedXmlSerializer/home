using LightInject;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// Custom service registry.  Modeled after <see cref="IServiceRegistry"/>.  If designed today, we would remove this
	/// interface with the LightInject version.  Perhaps in a future version we will do this as a breaking change.  For the
	/// default implementation, most calls pass through directly to the underlying LightInject equivalent.  For those
	/// operations that require a <see cref="ILifetime"/>, a <see cref="PerContainerLifetime"/> is used.
	/// </summary>
	/// <seealso cref="IServiceRegistry"/>
	public interface IServiceRepository
	{
		/// <seealso cref="IServiceRegistry.AvailableServices"/>
		IEnumerable<TypeInfo> AvailableServices { get; }

		/// <seealso cref="IServiceRegistry.Register(System.Type,System.Type)"/>
		IServiceRepository Register(Type serviceType, Type implementingType);

		/// <seealso cref="IServiceRegistry.Register(System.Type,System.Type,string)"/>
		IServiceRepository Register(Type serviceType, Type implementingType, string serviceName);

		/// <seealso cref="IServiceRegistry.Register{TService,TImplementation}()"/>
		IServiceRepository Register<TService, TImplementation>() where TImplementation : TService;

		/// <seealso cref="IServiceRegistry.Register{TService,TImplementation}(string)"/>
		IServiceRepository Register<TService, TImplementation>(string serviceName) where TImplementation : TService;

		/// <seealso cref="IServiceRegistry.Register{TService}()"/>
		IServiceRepository Register<TService>();

		/// <seealso cref="IServiceRegistry.RegisterInstance{TService}(TService)"/>
		IServiceRepository RegisterInstance<TService>(TService instance);

		/// <seealso cref="IServiceRegistry.RegisterInstance{TService}(TService,string)"/>
		IServiceRepository RegisterInstance<TService>(TService instance, string serviceName);

		/// <seealso cref="IServiceRegistry.RegisterInstance(Type,object)"/>
		IServiceRepository RegisterInstance(Type serviceType, object instance);

		/// <seealso cref="IServiceRegistry.RegisterInstance(Type,object,string)"/>
		IServiceRepository RegisterInstance(Type serviceType, object instance, string serviceName);

		/// <seealso cref="IServiceRegistry.RegisterInstance(Type,object)"/>
		IServiceRepository Register(Type serviceType);

		/// <remarks>Uses a <see cref="PerContainerLifetime"/> instance for the lifetime.</remarks>
		/// <seealso cref="IServiceRegistry.Register{TService}(Func{IServiceFactory, TService},ILifetime)"/>
		IServiceRepository Register<TService>(Func<IServiceProvider, TService> factory);

		/// <seealso cref="IServiceRegistry.Register{TService}(Func{IServiceFactory, TService},ILifetime)"/>
		IServiceRepository Register<T, TService>(Func<IServiceProvider, T, TService> factory);

		/// <seealso cref="IServiceRegistry.Register{T,TService}(Func{IServiceFactory,T,TService},string)"/>
		IServiceRepository Register<T, TService>(Func<IServiceProvider, T, TService> factory, string serviceName);

		/// <seealso cref="IServiceRegistry.Register{T1,T2,TService}(Func{IServiceFactory,T1,T2,TService})"/>
		IServiceRepository Register<T1, T2, TService>(Func<IServiceProvider, T1, T2, TService> factory);

		/// <seealso cref="IServiceRegistry.Register{T1,T2,TService}(Func{IServiceFactory,T1,T2,TService},string)"/>
		IServiceRepository Register<T1, T2, TService>(Func<IServiceProvider, T1, T2, TService> factory,
		                                              string serviceName);

		/// <seealso cref="IServiceRegistry.Register{T1,T2,T3,TService}(Func{IServiceFactory,T1,T2,T3,TService})"/>
		IServiceRepository Register<T1, T2, T3, TService>(Func<IServiceProvider, T1, T2, T3, TService> factory);

		/// <seealso cref="IServiceRegistry.Register{T1,T2,T3,TService}(Func{IServiceFactory,T1,T2,T3,TService},string)"/>
		IServiceRepository Register<T1, T2, T3, TService>(Func<IServiceProvider, T1, T2, T3, TService> factory,
		                                                  string serviceName);

		/// <seealso cref="IServiceRegistry.Register{T1,T2,T3,T4,TService}(Func{IServiceFactory,T1,T2,T3,T4,TService})"/>
		IServiceRepository Register<T1, T2, T3, T4, TService>(Func<IServiceProvider, T1, T2, T3, T4, TService> factory);

		/// <seealso cref="IServiceRegistry.Register{T1,T2,T3,T4,TService}(Func{IServiceFactory,T1,T2,T3,T4,TService},string)"/>
		IServiceRepository Register<T1, T2, T3, T4, TService>(Func<IServiceProvider, T1, T2, T3, T4, TService> factory,
		                                                      string serviceName);

		/// <seealso cref="IServiceRegistry.Register{TService}(Func{IServiceFactory,TService},string)"/>
		IServiceRepository Register<TService>(Func<IServiceProvider, TService> factory, string serviceName);

		/// <seealso cref="IServiceRegistry.RegisterFallback(System.Func{System.Type,string,bool},System.Func{LightInject.ServiceRequest,object})"/>
		IServiceRepository RegisterFallback(Func<Type, bool> predicate, Func<Type, object> factory);

		/// <seealso cref="IServiceRegistry.RegisterConstructorDependency{TDependency}(System.Func{LightInject.IServiceFactory,System.Reflection.ParameterInfo,TDependency})"/>
		IServiceRepository RegisterConstructorDependency<TDependency>(
			Func<IServiceProvider, ParameterInfo, TDependency> factory);

		/// <seealso cref="IServiceRegistry.RegisterConstructorDependency{TDependency}(System.Func{LightInject.IServiceFactory,System.Reflection.ParameterInfo,object[],TDependency})"/>
		IServiceRepository RegisterConstructorDependency<TDependency>(
			Func<IServiceProvider, ParameterInfo, object[], TDependency> factory);

		/// <seealso cref="IServiceRegistry.Decorate(System.Type,System.Type)"/>
		IServiceRepository Decorate(Type serviceType, Type decoratorType);

		/// <seealso cref="IServiceRegistry.Decorate{TService,TDecorator}"/>
		IServiceRepository Decorate<TService, TDecorator>() where TDecorator : TService;

		/// <seealso cref="IServiceRegistry.Decorate{TService}"/>
		IServiceRepository Decorate<TService>(Func<IServiceProvider, TService, TService> factory);
	}
}