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

using ExtendedXmlSerializer.ContentModel.Reflection;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	class TypeIdentity<T> : ITypeIdentity where T : Attribute
	{
		readonly Func<T, Key> _identity;

		public TypeIdentity(Func<T, Key> identity) => _identity = identity;

		public Key? Get(TypeInfo parameter)
			=> parameter.IsDefined(typeof(T)) ? (Key?)_identity(parameter.GetCustomAttribute<T>()) : null;
	}

	sealed class TypeIdentity : ITypeIdentity
	{
		public static TypeIdentity Default { get; } = new TypeIdentity();
		TypeIdentity() : this(XmlRootIdentity.Default, XmlTypeIdentity.Default) {}

		readonly ITypeIdentity _root;
		readonly ITypeIdentity _type;

		public TypeIdentity(ITypeIdentity root, ITypeIdentity type)
		{
			_root = root;
			_type = type;
		}

		public Key? Get(TypeInfo parameter) => _root.Get(parameter) ?? _type.Get(parameter);
	}
}