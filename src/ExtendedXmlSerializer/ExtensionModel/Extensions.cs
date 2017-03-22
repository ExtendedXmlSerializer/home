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
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public static class Extensions
	{
		readonly static Func<Type, bool>
			IsSatisfiedBy = new ActivatingTypeSpecification(PublicConstructorLocator.Default).IsSatisfiedBy;

		public static IServiceRepository RegisterAsStart<TItem, TStart>(this IServiceRepository @this)
			where TStart : IStart<TItem> => @this.RegisterWithDependencies<IStart<TItem>, TStart>();

		public static IServiceRepository RegisterAsFinish<TItem, TFinish>(this IServiceRepository @this)
			where TFinish : IFinish<TItem> => @this.RegisterWithDependencies<IFinish<TItem>, TFinish>();

		public static IServiceRepository RegisterWithDependencies<T>(this IServiceRepository @this)
			=> @this.Register<T>().WithDependencies(typeof(T));

		public static IServiceRepository RegisterWithDependencies<TFrom, TTo>(this IServiceRepository @this) where TTo : TFrom
			=> @this.Register<TFrom, TTo>().WithDependencies(typeof(TTo));

		static IServiceRepository WithDependencies(this IServiceRepository @this, Type type)
			=> Constructors.Default.Get(type)
			               .SelectMany(x => x.GetParameters().Select(y => y.ParameterType))
			               .Where(IsSatisfiedBy)
			               .Aggregate(@this, (repository, t) => repository.Register(t));

		public static IServiceRepository RegisterAsSet<TItem, TTo>(this IServiceRepository @this)
			where TTo : IEnumerable<TItem>
			=> @this.Register<TTo>()
			        .Register(provider => provider.GetInstance<TTo>().ToArray());
	}
}