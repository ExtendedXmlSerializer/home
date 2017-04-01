// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.LightInject;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel
{
	class Services : IServices
	{
		readonly IServiceContainer _container;
		readonly IServiceRegistry _registry;

		public Services(IServiceContainer container) : this(container, container) {}

		public Services(IServiceContainer container, IServiceRegistry registry)
		{
			_container = container;
			_registry = registry;
		}


		public IEnumerable<TypeInfo> AvailableServices => _registry.AvailableServices.Select(x => x.ServiceType.GetTypeInfo())
		;

		public IServiceRepository Register(Type serviceType, Type implementingType)
			=> new Services(_container, _registry.Register(serviceType, implementingType));

		public IServiceRepository Register(Type serviceType, Type implementingType, string serviceName)
			=> new Services(_container, _registry.Register(serviceType, implementingType, serviceName));

		public IServiceRepository Register<TService, TImplementation>() where TImplementation : TService
			=> new Services(_container, _registry.Register<TService, TImplementation>(new PerContainerLifetime()));

		public IServiceRepository Register<TService, TImplementation>(string serviceName) where TImplementation : TService
			=> new Services(_container, _registry.Register<TService, TImplementation>(serviceName));

		public IServiceRepository Register<TService>()
			=> new Services(_container, _registry.Register<TService>(new PerContainerLifetime()));

		public IServiceRepository RegisterInstance<TService>(TService instance)
			=> new Services(_container, _registry.RegisterInstance(instance));

		public IServiceRepository RegisterInstance<TService>(TService instance, string serviceName)
			=> new Services(_container, _registry.RegisterInstance(instance, serviceName));

		public IServiceRepository RegisterInstance(Type serviceType, object instance)
			=> new Services(_container, _registry.RegisterInstance(serviceType, instance));

		public IServiceRepository RegisterInstance(Type serviceType, object instance, string serviceName)
			=> new Services(_container, _registry.RegisterInstance(serviceType, instance, serviceName));

		public IServiceRepository Register(Type serviceType) => new Services(_container, _registry.Register(serviceType));

		public IServiceRepository Register<TService>(Func<IServiceProvider, TService> factory)
			=> new Services(_container, _registry.Register(new Registration<TService>(factory).Get, new PerContainerLifetime()));

		sealed class Providers : ReferenceCache<IServiceFactory, IServiceProvider>
		{
			public static Providers Default { get; } = new Providers();
			Providers() : base(x => new Provider(x)) {}

			class Provider : IServiceProvider
			{
				readonly IServiceFactory _factory;

				public Provider(IServiceFactory factory)
				{
					_factory = factory;
				}

				public object GetService(Type serviceType) => _factory.Create(serviceType);

				public object GetInstance(Type serviceType) => _factory.GetInstance(serviceType);

				public object GetInstance(Type serviceType, object[] arguments) => _factory.GetInstance(serviceType, arguments);

				public object GetInstance(Type serviceType, string serviceName, object[] arguments)
					=> _factory.GetInstance(serviceType, serviceName, arguments);

				public object GetInstance(Type serviceType, string serviceName) => _factory.GetInstance(serviceType, serviceName);

				public object TryGetInstance(Type serviceType) => _factory.TryGetInstance(serviceType);

				public object TryGetInstance(Type serviceType, string serviceName)
					=> _factory.TryGetInstance(serviceType, serviceName);

				public IEnumerable<object> GetAllInstances(Type serviceType) => _factory.GetAllInstances(serviceType);

				public object Create(Type serviceType) => _factory.Create(serviceType);
			}
		}

		public IServiceRepository Register<T, TService>(Func<IServiceProvider, T, TService> factory)
			=> new Services(_container, _registry.Register<T, TService>(new Registration<T, TService>(factory).Get));

		public IServiceRepository Register<T, TService>(Func<IServiceProvider, T, TService> factory, string serviceName)
			=> new Services(_container, _registry.Register<T, TService>(new Registration<T, TService>(factory).Get, serviceName));

		public IServiceRepository Register<T1, T2, TService>(Func<IServiceProvider, T1, T2, TService> factory)
			=> new Services(_container, _registry.Register<T1, T2, TService>(new Registration<T1, T2, TService>(factory).Get));

		public IServiceRepository Register<T1, T2, TService>(Func<IServiceProvider, T1, T2, TService> factory,
		                                                     string serviceName)
			=>
				new Services(_container,
				             _registry.Register<T1, T2, TService>(new Registration<T1, T2, TService>(factory).Get, serviceName));

		public IServiceRepository Register<T1, T2, T3, TService>(Func<IServiceProvider, T1, T2, T3, TService> factory)
			=>
				new Services(_container,
				             _registry.Register<T1, T2, T3, TService>(new Registration<T1, T2, T3, TService>(factory).Get));

		public IServiceRepository Register<T1, T2, T3, TService>(Func<IServiceProvider, T1, T2, T3, TService> factory,
		                                                         string serviceName)
			=>
				new Services(_container,
				             _registry.Register<T1, T2, T3, TService>(new Registration<T1, T2, T3, TService>(factory).Get,
				                                                      serviceName));

		public IServiceRepository Register<T1, T2, T3, T4, TService>(Func<IServiceProvider, T1, T2, T3, T4, TService> factory)
			=>
				new Services(_container,
				             _registry.Register<T1, T2, T3, T4, TService>(new Registration<T1, T2, T3, T4, TService>(factory).Get));

		public IServiceRepository Register<T1, T2, T3, T4, TService>(Func<IServiceProvider, T1, T2, T3, T4, TService> factory,
		                                                             string serviceName)
			=>
				new Services(_container,
				             _registry.Register<T1, T2, T3, T4, TService>(new Registration<T1, T2, T3, T4, TService>(factory).Get,
				                                                          serviceName));

		public IServiceRepository Register<TService>(Func<IServiceProvider, TService> factory, string serviceName)
			=> new Services(_container, _registry.Register(new Registration<TService>(factory).Get, serviceName));

		public IServiceRepository RegisterFallback(Func<Type, bool> predicate, Func<Type, object> factory)
			=>
				new Services(_container,
				             _registry.RegisterFallback((type, s) => predicate(type), request => factory(request.ServiceType)));

		public IServiceRepository RegisterConstructorDependency<TDependency>(
			Func<IServiceProvider, ParameterInfo, TDependency> factory)
			=> new Services(_container, _registry.RegisterConstructorDependency(new Dependency<TDependency>(factory).Get));

		public IServiceRepository RegisterConstructorDependency<TDependency>(
			Func<IServiceProvider, ParameterInfo, object[], TDependency> factory)
			=> new Services(_container, _registry.RegisterConstructorDependency(new Arguments<TDependency>(factory).Get));

		public IServiceRepository Decorate(Type serviceType, Type decoratorType)
			=> new Services(_container, _registry.Decorate(serviceType, decoratorType));

		public IServiceRepository Decorate<TService, TDecorator>() where TDecorator : TService
			=> new Services(_container, _registry.Decorate<TService, TDecorator>());

		public IServiceRepository Decorate<TService>(Func<IServiceProvider, TService, TService> factory)
			=> new Services(_container, _registry.Decorate<TService>(new Decoration<TService>(factory).Get));

		public object Create(Type serviceType) => _container.Create(serviceType);

		public object GetService(Type serviceType) => _container.GetInstance(serviceType);

		public object GetInstance(Type serviceType) => _container.GetInstance(serviceType);

		public object GetInstance(Type serviceType, object[] arguments) => _container.GetInstance(serviceType, arguments);

		public object GetInstance(Type serviceType, string serviceName, object[] arguments)
			=> _container.GetInstance(serviceType, serviceName, arguments);

		public object GetInstance(Type serviceType, string serviceName) => _container.GetInstance(serviceType, serviceName);

		public object TryGetInstance(Type serviceType) => _container.TryGetInstance(serviceType);

		public object TryGetInstance(Type serviceType, string serviceName)
			=> _container.TryGetInstance(serviceType, serviceName);

		public IEnumerable<object> GetAllInstances(Type serviceType) => _container.GetAllInstances(serviceType);


		public void Dispose() => _container.Dispose();

		class Arguments<T>
		{
			readonly Func<IServiceProvider, ParameterInfo, object[], T> _factory;

			public Arguments(Func<IServiceProvider, ParameterInfo, object[], T> factory)
			{
				_factory = factory;
			}

			public T Get(IServiceFactory factory, ParameterInfo parameter, object[] arguments)
				=> _factory(Providers.Default.Get(factory), parameter, arguments);
		}

		class Decoration<T>
		{
			readonly Func<IServiceProvider, T, T> _factory;

			public Decoration(Func<IServiceProvider, T, T> factory)
			{
				_factory = factory;
			}

			public T Get(IServiceFactory factory, T parameter) => _factory(Providers.Default.Get(factory), parameter);
		}

		class Dependency<T>
		{
			readonly Func<IServiceProvider, ParameterInfo, T> _factory;

			public Dependency(Func<IServiceProvider, ParameterInfo, T> factory)
			{
				_factory = factory;
			}

			public T Get(IServiceFactory factory, ParameterInfo parameter) => _factory(Providers.Default.Get(factory), parameter);
		}

		class Registration<T>
		{
			readonly Func<IServiceProvider, T> _factory;

			public Registration(Func<IServiceProvider, T> factory)
			{
				_factory = factory;
			}

			public T Get(IServiceFactory factory) => _factory(Providers.Default.Get(factory));
		}

		class Registration<T, TService>
		{
			readonly Func<IServiceProvider, T, TService> _factory;

			public Registration(Func<IServiceProvider, T, TService> factory)
			{
				_factory = factory;
			}

			public TService Get(IServiceFactory factory, T parameter) => _factory(Providers.Default.Get(factory), parameter);
		}

		class Registration<T1, T2, TService>
		{
			readonly Func<IServiceProvider, T1, T2, TService> _factory;

			public Registration(Func<IServiceProvider, T1, T2, TService> factory)
			{
				_factory = factory;
			}

			public TService Get(IServiceFactory factory, T1 first, T2 second)
				=> _factory(Providers.Default.Get(factory), first, second);
		}

		class Registration<T1, T2, T3, TService>
		{
			readonly Func<IServiceProvider, T1, T2, T3, TService> _factory;

			public Registration(Func<IServiceProvider, T1, T2, T3, TService> factory)
			{
				_factory = factory;
			}

			public TService Get(IServiceFactory factory, T1 first, T2 second, T3 third)
				=> _factory(Providers.Default.Get(factory), first, second, third);
		}

		class Registration<T1, T2, T3, T4, TService>
		{
			readonly Func<IServiceProvider, T1, T2, T3, T4, TService> _factory;

			public Registration(Func<IServiceProvider, T1, T2, T3, T4, TService> factory)
			{
				_factory = factory;
			}

			public TService Get(IServiceFactory factory, T1 first, T2 second, T3 third, T4 forth)
				=> _factory(Providers.Default.Get(factory), first, second, third, forth);
		}
	}
}