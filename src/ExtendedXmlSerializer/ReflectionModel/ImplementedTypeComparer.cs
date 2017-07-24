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

using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ImplementedTypeComparer : ITypeComparer
	{
		public static ImplementedTypeComparer Default { get; } = new ImplementedTypeComparer();
		ImplementedTypeComparer() : this(InterfaceIdentities.Default, TypeDefinitionIdentityComparer.Default) {}

		readonly IInterfaceIdentities _interfaces;
		readonly ITypeComparer _identity;

		public ImplementedTypeComparer(IInterfaceIdentities interfaces, ITypeComparer identity)
		{
			_interfaces = interfaces;
			_identity = identity;
		}

		public bool Equals(TypeInfo x, TypeInfo y)
		{
			var left = x.IsInterface;
			if (left != y.IsInterface)
			{
				var @interface = left ? x : y;
				var implementation = left ? y : x;
				var contains = _interfaces.Get(implementation).Contains(@interface.GUID);
				return contains;
			}
			var result = _identity.Equals(x, y);
			return result;
		}

		public int GetHashCode(TypeInfo obj) => 0;
	}
}