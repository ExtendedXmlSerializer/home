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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Namespacing;

namespace ExtendedXmlSerialization.ContentModel.Extensions
{
	class ObjectNamespaces : IObjectNamespaces
	{
		readonly static Func<TypeInfo, string> Identities = Identifiers.Default.Get;

		readonly IMembers _members;
		readonly Func<TypeInfo, string> _names;
		readonly Func<string, Namespace> _namespaces;

		public ObjectNamespaces(IMembers members) : this(members, Identities, new Namespaces().Get) {}

		public ObjectNamespaces(IMembers members, Func<TypeInfo, string> names, Func<string, Namespace> namespaces)
		{
			_members = members;
			_names = names;
			_namespaces = namespaces;
		}

		public ImmutableArray<Namespace> Get(object parameter)
		{
			var types = new ObjectTypeWalker(_members, parameter).Get().Distinct().ToImmutableArray();
			var items = types.Length > 1 ? types.Add(Defaults.FrameworkType) : types;
			var result = items
				.Select(_names)
				.Distinct()
				.Select(_namespaces)
				.ToImmutableArray();
			return result;
		}
	}
}