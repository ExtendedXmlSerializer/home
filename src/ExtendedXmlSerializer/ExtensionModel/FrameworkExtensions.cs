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

using ExtendedXmlSerializer.ExtensionModel.Services;
using System;

namespace ExtendedXmlSerializer.ExtensionModel
{
	static class FrameworkExtensions
	{
		public static IServiceRepository RegisterDefinition<T>(this IServiceRepository @this)
		{
			var to = typeof(T).GetGenericTypeDefinition();
			return @this.Register(to)
			            .RegisterDependencies(to);
		}

		/*public static IServiceRepository RegisterSingleton<T>(this IServiceRepository @this)
		{
			var to = typeof(T).GetGenericTypeDefinition();
			return @this.Register(to)
			            .RegisterDependencies(to);
		}*/

		public static IServiceRepository RegisterDefinition<TFrom, TTo>(this IServiceRepository @this) where TTo : TFrom
		{
			var to = typeof(TTo).GetGenericTypeDefinition();
			return @this.Register(to)
			            .Register(typeof(TFrom).GetGenericTypeDefinition(), to)
			            .RegisterDependencies(to);
		}

		public static IServiceRepository DecorateWithDependencies<TFrom, TTo>(this IServiceRepository @this) where TTo : TFrom
			=> @this.Decorate<TFrom, TTo>()
			        .RegisterDependencies(typeof(TTo));

		public static IServiceRepository DecorateDefinition<TFrom, TTo>(this IServiceRepository @this) where TTo : TFrom
		{
			var to = typeof(TTo).GetGenericTypeDefinition();
			return @this.Register(to)
			            .Decorate(typeof(TFrom).GetGenericTypeDefinition(), to)
			            .RegisterDependencies(to);
		}

		public static IServiceRepository RegisterWithDependencies<T>(this IServiceRepository @this)
			=> @this.Register<T>()
			        .RegisterDependencies(typeof(T));

		public static IServiceRepository RegisterWithDependencies<TFrom, TTo>(this IServiceRepository @this) where TTo : TFrom
			=> @this.Register<TFrom, TTo>()
			        .RegisterDependencies(typeof(TTo));

		public static IServiceRepository RegisterDependencies(this IServiceRepository @this, Type type)
			=> new RegisterDependencies(type).Get(@this);
	}
}