// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Collections.Concurrent;
using System.Reflection;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Processing
{
	public interface IAddMethodLocator
	{
		MethodInfo Locate(Type type, Type elementType);
	}

	public sealed class AddMethodLocator : ConcurrentDictionary<Type, MethodInfo>, IAddMethodLocator
	{
		const string Add = "Add";
		
		public static AddMethodLocator Default { get; } = new AddMethodLocator();
		AddMethodLocator() {}

		public MethodInfo Locate(Type type, Type elementType)
		{
			return GetOrAdd(type, t => Get(type, elementType));
		}

		static MethodInfo Get(Type type, Type elementType)
		{
			foreach (var candidate in AllInterfaces.Instance.Yield(type))
			{
				var method = candidate.GetMethod(Add);
				var parameters = method?.GetParameters();
				if (parameters?.Length == 1 && elementType.IsAssignableFrom(parameters[0].ParameterType))
				{
					return method;						
				}
			}
			return null;
		}
	}
}