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

using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	static class Support<T>
	{
		public static TypeInfo Key { get; } = typeof(T).GetTypeInfo();

		public static Func<T> New { get; } = DefaultActivators.Default.New<T>;

		public static Func<T> NewOrSingleton { get; } = Activators.Default.New<T>;

		public static T[] Empty { get; } = new T[]{};
	}

	static class StaticTypeChecingExtensions
	{
		public static IWrap<T1, T2> With<T1, T2>(this T1 @this, A<T2> _) => new Wrap<T1, T2>(@this);
	}

	// ReSharper disable once UnusedTypeParameter
	public interface IWrap<out T1, T2> : ISource<T1> {}

	sealed class Wrap<T1, T2> : FixedInstanceSource<T1>, IWrap<T1, T2>
	{
		public Wrap(T1 instance) : base(instance) {}
	}

	public interface ITypes : ISource<TypeInfo> { }

	public sealed class A<T> : FixedInstanceSource<TypeInfo>, ITypes
	{
		public static A<T> Default { get; } = new A<T>();
		A() : base(typeof(T).GetTypeInfo()) {}
	}
}