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
	// ATTRIBUTION: Based on https://github.com/seesharper/LightInject/blob/master/src/LightInject/LightInject.cs#L88
	// Basically using this as a decorated contract for now.
	public interface IServices : IServiceProvider, IDisposable
	{
		IEnumerable<ServiceRegistration> AvailableServices { get; }

		IServices Register(Type serviceType, Type implementingType);

		/*IServiceRegistry Register(Type serviceType, Type implementingType, ILifetime lifetime);*/

		IServices Register(Type serviceType, Type implementingType, string serviceName);

		/*IServiceRegistry Register(Type serviceType, Type implementingType, string serviceName, ILifetime lifetime);*/

		IServices Register<TService, TImplementation>() where TImplementation : TService;

		IServices Register<TService, TImplementation>(string serviceName) where TImplementation : TService;

		IServices Register<TService>();

		IServices RegisterInstance<TService>(TService instance);
		IServices RegisterInstance<TService>(TService instance, string serviceName);
		IServices RegisterInstance(Type serviceType, object instance);

		IServices RegisterInstance(Type serviceType, object instance, string serviceName);


		IServices Register(Type serviceType);

		IServices Register<TService>(Func<IServiceFactory, TService> factory);
		IServices Register<T, TService>(Func<IServiceFactory, T, TService> factory);
		IServices Register<T, TService>(Func<IServiceFactory, T, TService> factory, string serviceName);
		IServices Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory);
		IServices Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory, string serviceName);
		IServices Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory);

		IServices Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory,
		                                                string serviceName);

		IServices Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory);

		IServices Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory,
		                                                    string serviceName);

		IServices Register<TService>(Func<IServiceFactory, TService> factory, string serviceName);
		IServices RegisterFallback(Func<Type, bool> predicate, Func<Type, object> factory);


		/*IServiceRegistry Register<TService, TImplementation>(ILifetime lifetime) where TImplementation : TService;
		IServiceRegistry Register<TService, TImplementation>(string serviceName, ILifetime lifetime) where TImplementation : TService;
		IServiceRegistry Register<TService>(ILifetime lifetime);
		IServiceRegistry Register(Type serviceType, ILifetime lifetime);
		IServiceRegistry Register<TService>(Func<IServiceFactory, TService> factory, ILifetime lifetime);
		IServiceRegistry Register<TService>(Func<IServiceFactory, TService> factory, string serviceName, ILifetime lifetime);	 
		IServiceRegistry RegisterFallback(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory, ILifetime lifetime);

					IServiceRegistry Decorate(Type serviceType, Type decoratorType, Func<ServiceRegistration, bool> predicate);
					IServiceRegistry Decorate(DecoratorRegistration decoratorRegistration);
					IServiceRegistry Override(Func<ServiceRegistration, bool> serviceSelector, Func<IServiceFactory, ServiceRegistration, ServiceRegistration> serviceRegistrationFactory);
					*/
		IServices RegisterConstructorDependency<TDependency>(Func<IServiceFactory, ParameterInfo, TDependency> factory);

		IServices RegisterConstructorDependency<TDependency>(
			Func<IServiceFactory, ParameterInfo, object[], TDependency> factory);


		IServices Decorate(Type serviceType, Type decoratorType);

		IServices Decorate<TService, TDecorator>() where TDecorator : TService;
		IServices Decorate<TService>(Func<IServiceFactory, TService, TService> factory);
	}
}