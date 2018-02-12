// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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

namespace ExtendedXmlSerializer.ExtensionModel.Services
{
	// ATTRIBUTION: Based on https://github.com/seesharper/LightInject/blob/master/src/LightInject/LightInject.cs#L88
	// Basically using this as a decorated contract for now.
	public interface IServiceRepository
	{
		IEnumerable<TypeInfo> AvailableServices { get; }

		IServiceRepository Register(Type serviceType, Type implementingType);

		IServiceRepository Register(Type serviceType, Type implementingType, string serviceName);

		IServiceRepository Register<TService, TImplementation>() where TImplementation : TService;

		IServiceRepository Register<TService, TImplementation>(string serviceName) where TImplementation : TService;

		IServiceRepository Register<TService>();

		IServiceRepository RegisterInstance<TService>(TService instance);
		IServiceRepository RegisterInstance<TService>(TService instance, string serviceName);
		IServiceRepository RegisterInstance(Type serviceType, object instance);

		IServiceRepository RegisterInstance(Type serviceType, object instance, string serviceName);


		IServiceRepository Register(Type serviceType);

		IServiceRepository Register<TService>(Func<IServiceProvider, TService> factory);
		IServiceRepository Register<T, TService>(Func<IServiceProvider, T, TService> factory);
		IServiceRepository Register<T, TService>(Func<IServiceProvider, T, TService> factory, string serviceName);
		IServiceRepository Register<T1, T2, TService>(Func<IServiceProvider, T1, T2, TService> factory);
		IServiceRepository Register<T1, T2, TService>(Func<IServiceProvider, T1, T2, TService> factory, string serviceName);
		IServiceRepository Register<T1, T2, T3, TService>(Func<IServiceProvider, T1, T2, T3, TService> factory);

		IServiceRepository Register<T1, T2, T3, TService>(Func<IServiceProvider, T1, T2, T3, TService> factory,
		                                                  string serviceName);

		IServiceRepository Register<T1, T2, T3, T4, TService>(Func<IServiceProvider, T1, T2, T3, T4, TService> factory);

		IServiceRepository Register<T1, T2, T3, T4, TService>(Func<IServiceProvider, T1, T2, T3, T4, TService> factory,
		                                                      string serviceName);

		IServiceRepository Register<TService>(Func<IServiceProvider, TService> factory, string serviceName);
		IServiceRepository RegisterFallback(Func<Type, bool> predicate, Func<Type, object> factory);


		IServiceRepository RegisterConstructorDependency<TDependency>(
			Func<IServiceProvider, ParameterInfo, TDependency> factory);

		IServiceRepository RegisterConstructorDependency<TDependency>(
			Func<IServiceProvider, ParameterInfo, object[], TDependency> factory);


		IServiceRepository Decorate(Type serviceType, Type decoratorType);

		IServiceRepository Decorate<TService, TDecorator>() where TDecorator : TService;
		IServiceRepository Decorate<TService>(Func<IServiceProvider, TService, TService> factory);
	}
}