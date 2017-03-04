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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ExtensionModel
{
	public static class Extensions
	{
		public static IServiceRepository RegisterInstanceByConvention(this IServiceRepository registry, object service)
		{
			var serviceType = service.GetType();
			var inherited = AllInterfaces.Default.Get(serviceType.GetTypeInfo().BaseType.GetTypeInfo());
			var specific = AllInterfaces.Default.Get(serviceType.GetTypeInfo());
			var interfaces = specific.Except(inherited).ToArray();
			var convention = interfaces.Length == 1 ? interfaces[0].AsType() : Name(serviceType, interfaces);
			if (convention != null)
			{
				registry.RegisterInstance(convention, service);
			}

			registry.RegisterInstance(serviceType, service);
			return registry;
		}

		static Type Name(Type owner, TypeInfo[] interfaces)
		{
			var name = $"I{owner.Name}";
			var length = interfaces.Length;
			for (var i = 0; i < length; i++)
			{
				var type = interfaces[i];
				if (type.Name.Equals(name))
				{
					return type.AsType();
				}
			}
			return null;
		}
	}
}