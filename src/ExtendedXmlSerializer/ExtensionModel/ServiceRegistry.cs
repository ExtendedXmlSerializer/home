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
using System.Reflection;
using LightInject;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ServiceRegistry : IServiceRegistry
	{
		readonly IServiceContainer _container;
		readonly LightInject.IServiceRegistry _registry;

		public ServiceRegistry(IServiceContainer container) : this(container, container) {}

		public ServiceRegistry(IServiceContainer container, LightInject.IServiceRegistry registry)
		{
			_container = container;
			_registry = registry;
		}


		public IEnumerable<ServiceRegistration> AvailableServices => _registry.AvailableServices;

		public IServiceRegistry Register(Type serviceType, Type implementingType)
			=> new ServiceRegistry(_container, _registry.Register(serviceType, implementingType));

		public IServiceRegistry Register(Type serviceType, Type implementingType, ILifetime lifetime)
			=> new ServiceRegistry(_container, _registry.Register(serviceType, implementingType, lifetime));

		public IServiceRegistry Register(Type serviceType, Type implementingType, string serviceName)
			=> new ServiceRegistry(_container, _registry.Register(serviceType, implementingType, serviceName));

		public IServiceRegistry Register(Type serviceType, Type implementingType, string serviceName, ILifetime lifetime)
			=> new ServiceRegistry(_container, _registry.Register(serviceType, implementingType, serviceName, lifetime));

		public IServiceRegistry Register<TService, TImplementation>() where TImplementation : TService
			=> new ServiceRegistry(_container, _registry.Register<TService, TImplementation>());

		public IServiceRegistry Register<TService, TImplementation>(string serviceName) where TImplementation : TService
			=> new ServiceRegistry(_container, _registry.Register<TService, TImplementation>(serviceName));

		public IServiceRegistry Register<TService>() => new ServiceRegistry(_container, _registry.Register<TService>());

		public IServiceRegistry RegisterInstance<TService>(TService instance)
			=> new ServiceRegistry(_container, _registry.RegisterInstance(instance));

		public IServiceRegistry RegisterInstance<TService>(TService instance, string serviceName)
			=> new ServiceRegistry(_container, _registry.RegisterInstance(instance, serviceName));

		public IServiceRegistry RegisterInstance(Type serviceType, object instance)
			=> new ServiceRegistry(_container, _registry.RegisterInstance(serviceType, instance));

		public IServiceRegistry RegisterInstance(Type serviceType, object instance, string serviceName)
			=> new ServiceRegistry(_container, _registry.RegisterInstance(serviceType, instance, serviceName));

		public IServiceRegistry Register(Type serviceType) => new ServiceRegistry(_container, _registry.Register(serviceType));

		public IServiceRegistry Register<TService>(Func<IServiceFactory, TService> factory)
			=> new ServiceRegistry(_container, _registry.Register(factory));

		public IServiceRegistry Register<T, TService>(Func<IServiceFactory, T, TService> factory)
			=> new ServiceRegistry(_container, _registry.Register(factory));

		public IServiceRegistry Register<T, TService>(Func<IServiceFactory, T, TService> factory, string serviceName)
			=> new ServiceRegistry(_container, _registry.Register(factory, serviceName));

		public IServiceRegistry Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory)
			=> new ServiceRegistry(_container, _registry.Register(factory));

		public IServiceRegistry Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory, string serviceName)
			=> new ServiceRegistry(_container, _registry.Register(factory, serviceName));

		public IServiceRegistry Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory)
			=> new ServiceRegistry(_container, _registry.Register(factory));

		public IServiceRegistry Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory,
		                                                       string serviceName)
			=> new ServiceRegistry(_container, _registry.Register(factory, serviceName));

		public IServiceRegistry Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory)
			=> new ServiceRegistry(_container, _registry.Register(factory));

		public IServiceRegistry Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory,
		                                                           string serviceName)
			=> new ServiceRegistry(_container, _registry.Register(factory, serviceName));

		public IServiceRegistry Register<TService>(Func<IServiceFactory, TService> factory, string serviceName)
			=> new ServiceRegistry(_container, _registry.Register(factory, serviceName));

		public IServiceRegistry RegisterFallback(Func<Type, bool> predicate, Func<Type, object> factory)
			=>
				new ServiceRegistry(_container,
				                    _registry.RegisterFallback((type, s) => predicate(type), request => factory(request.ServiceType)));

		public IServiceRegistry RegisterConstructorDependency<TDependency>(
			Func<IServiceFactory, ParameterInfo, TDependency> factory)
			=> new ServiceRegistry(_container, _registry.RegisterConstructorDependency(factory));

		public IServiceRegistry RegisterConstructorDependency<TDependency>(
			Func<IServiceFactory, ParameterInfo, object[], TDependency> factory)
			=> new ServiceRegistry(_container, _registry.RegisterConstructorDependency(factory));

		public IServiceRegistry Decorate(Type serviceType, Type decoratorType)
			=> new ServiceRegistry(_container, _registry.Decorate(serviceType, decoratorType));

		public IServiceRegistry Decorate<TService, TDecorator>() where TDecorator : TService
			=> new ServiceRegistry(_container, _registry.Decorate<TService, TDecorator>());

		public IServiceRegistry Decorate<TService>(Func<IServiceFactory, TService, TService> factory)
			=> new ServiceRegistry(_container, _registry.Decorate(factory));

		public object GetService(Type serviceType) => _container.GetInstance(serviceType);
	}
}