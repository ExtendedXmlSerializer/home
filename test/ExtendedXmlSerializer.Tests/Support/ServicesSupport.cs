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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IServiceProvider = ExtendedXmlSerializer.ExtensionModel.IServiceProvider;

namespace ExtendedXmlSerializer.Tests.Support
{
	class ServicesSupport : IServices
	{
		readonly IServices _services;

		public ServicesSupport() : this(DefaultExtensions.Default.ToArray()) {}

		public ServicesSupport(params ISerializerExtension[] extensions)
			: this(ServicesFactory.Default.Get(new ConfigurationContainer(extensions).Root)) {}

		public ServicesSupport(IServices services)
		{
			_services = services;
		}

		public void Dispose() => _services.Dispose();

		public IEnumerable<TypeInfo> AvailableServices => _services.AvailableServices;

		public IServiceRepository Register(Type serviceType, Type implementingType)
			=> _services.Register(serviceType, implementingType);

		public IServiceRepository Register(Type serviceType, Type implementingType, string serviceName)
			=> _services.Register(serviceType, implementingType, serviceName);

		public IServiceRepository Register<TService, TImplementation>() where TImplementation : TService
			=> _services.Register<TService, TImplementation>();

		public IServiceRepository Register<TService, TImplementation>(string serviceName) where TImplementation : TService
			=> _services.Register<TService, TImplementation>(serviceName);

		public IServiceRepository Register<TService>() => _services.Register<TService>();

		public IServiceRepository RegisterInstance<TService>(TService instance) => _services.RegisterInstance(instance);

		public IServiceRepository RegisterInstance<TService>(TService instance, string serviceName)
			=> _services.RegisterInstance(instance, serviceName);

		public IServiceRepository RegisterInstance(Type serviceType, object instance)
			=> _services.RegisterInstance(serviceType, instance);

		public IServiceRepository RegisterInstance(Type serviceType, object instance, string serviceName)
			=> _services.RegisterInstance(serviceType, instance, serviceName);

		public IServiceRepository Register(Type serviceType) => _services.Register(serviceType);
		public IServiceRepository Register<TService>(Func<IServiceProvider, TService> factory) => _services.Register(factory);

		public IServiceRepository Register<T, TService>(Func<IServiceProvider, T, TService> factory)
			=> _services.Register(factory);

		public IServiceRepository Register<T, TService>(Func<IServiceProvider, T, TService> factory, string serviceName)
			=> _services.Register(factory, serviceName);

		public IServiceRepository Register<T1, T2, TService>(Func<IServiceProvider, T1, T2, TService> factory)
			=> _services.Register(factory);

		public IServiceRepository Register<T1, T2, TService>(Func<IServiceProvider, T1, T2, TService> factory,
		                                                     string serviceName) => _services.Register(factory, serviceName);

		public IServiceRepository Register<T1, T2, T3, TService>(Func<IServiceProvider, T1, T2, T3, TService> factory)
			=> _services.Register(factory);

		public IServiceRepository Register<T1, T2, T3, TService>(Func<IServiceProvider, T1, T2, T3, TService> factory,
		                                                         string serviceName)
			=> _services.Register(factory, serviceName);

		public IServiceRepository Register<T1, T2, T3, T4, TService>(Func<IServiceProvider, T1, T2, T3, T4, TService> factory)
			=> _services.Register(factory);

		public IServiceRepository Register<T1, T2, T3, T4, TService>(Func<IServiceProvider, T1, T2, T3, T4, TService> factory,
		                                                             string serviceName)
			=> _services.Register(factory, serviceName);

		public IServiceRepository Register<TService>(Func<IServiceProvider, TService> factory, string serviceName)
			=> _services.Register(factory, serviceName);

		public IServiceRepository RegisterFallback(Func<Type, bool> predicate, Func<Type, object> factory)
			=> _services.RegisterFallback(predicate, factory);

		public IServiceRepository RegisterConstructorDependency<TDependency>(
			Func<IServiceProvider, ParameterInfo, TDependency> factory) => _services.RegisterConstructorDependency(factory);

		public IServiceRepository RegisterConstructorDependency<TDependency>(
			Func<IServiceProvider, ParameterInfo, object[], TDependency> factory)
			=> _services.RegisterConstructorDependency(factory);

		public IServiceRepository Decorate(Type serviceType, Type decoratorType)
			=> _services.Decorate(serviceType, decoratorType);

		public IServiceRepository Decorate<TService, TDecorator>() where TDecorator : TService
			=> _services.Decorate<TService, TDecorator>();

		public IServiceRepository Decorate<TService>(Func<IServiceProvider, TService, TService> factory)
			=> _services.Decorate(factory);

		public object GetInstance(Type serviceType) => _services.GetInstance(serviceType);

		public object GetInstance(Type serviceType, object[] arguments) => _services.GetInstance(serviceType, arguments);

		public object GetInstance(Type serviceType, string serviceName, object[] arguments)
			=> _services.GetInstance(serviceType, serviceName, arguments);

		public object GetInstance(Type serviceType, string serviceName) => _services.GetInstance(serviceType, serviceName);

		public object TryGetInstance(Type serviceType) => _services.TryGetInstance(serviceType);

		public object TryGetInstance(Type serviceType, string serviceName)
			=> _services.TryGetInstance(serviceType, serviceName);

		public IEnumerable<object> GetAllInstances(Type serviceType) => _services.GetAllInstances(serviceType);

		public object Create(Type serviceType) => _services.Create(serviceType);
		public object GetService(Type serviceType) => _services.GetService(serviceType);
	}
}