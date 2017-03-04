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

using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ExtensionModel
{
	public static class ServiceProviderExtensions
	{
		public static TService GetInstance<TService>(this IServiceProvider factory)
			=> (TService) factory.GetInstance(typeof(TService));

		public static TService GetInstance<TService>(this IServiceProvider factory, string serviceName)
			=> (TService) factory.GetInstance(typeof(TService), serviceName);

		public static TService GetInstance<T, TService>(this IServiceProvider factory, T value)
			=> (TService) factory.GetInstance(typeof(TService), new object[] {value});

		public static TService GetInstance<T, TService>(this IServiceProvider factory, T value, string serviceName)
			=> (TService) factory.GetInstance(typeof(TService), serviceName, new object[] {value});

		public static TService GetInstance<T1, T2, TService>(this IServiceProvider factory, T1 arg1, T2 arg2)
			=> (TService) factory.GetInstance(typeof(TService), new object[] {arg1, arg2});

		public static TService GetInstance<T1, T2, TService>(this IServiceProvider factory, T1 arg1, T2 arg2,
		                                                     string serviceName)
			=> (TService) factory.GetInstance(typeof(TService), serviceName, new object[] {arg1, arg2});

		public static TService GetInstance<T1, T2, T3, TService>(this IServiceProvider factory, T1 arg1, T2 arg2, T3 arg3)
			=> (TService) factory.GetInstance(typeof(TService), new object[] {arg1, arg2, arg3});

		public static TService GetInstance<T1, T2, T3, TService>(this IServiceProvider factory, T1 arg1, T2 arg2, T3 arg3,
		                                                         string serviceName)
			=> (TService) factory.GetInstance(typeof(TService), serviceName, new object[] {arg1, arg2, arg3});

		public static TService GetInstance<T1, T2, T3, T4, TService>(this IServiceProvider factory, T1 arg1, T2 arg2, T3 arg3,
		                                                             T4 arg4)
			=> (TService) factory.GetInstance(typeof(TService), new object[] {arg1, arg2, arg3, arg4});

		public static TService GetInstance<T1, T2, T3, T4, TService>(this IServiceProvider factory, T1 arg1, T2 arg2, T3 arg3,
		                                                             T4 arg4, string serviceName)
			=> (TService) factory.GetInstance(typeof(TService), serviceName, new object[] {arg1, arg2, arg3, arg4});

		public static TService TryGetInstance<TService>(this IServiceProvider factory)
			=> (TService) factory.TryGetInstance(typeof(TService));

		public static TService TryGetInstance<TService>(this IServiceProvider factory, string serviceName)
			=> (TService) factory.TryGetInstance(typeof(TService), serviceName);

		public static IEnumerable<TService> GetAllInstances<TService>(this IServiceProvider factory)
			=> factory.GetInstance<IEnumerable<TService>>();

		public static TService Create<TService>(this IServiceProvider factory) where TService : class
			=> (TService) factory.Create(typeof(TService));

		public static T AsDependency<T>(this System.IServiceProvider @this, object _, ParameterInfo __) => @this.Get<T>();
	}
}