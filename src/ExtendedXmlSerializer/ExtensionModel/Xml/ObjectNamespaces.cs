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
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.ContentModel.Xml.Namespacing;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.TypeModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ObjectNamespaces : IObjectNamespaces
	{
		readonly static Func<TypeInfo, string> Identities = Identifiers.Default.Get;

		readonly ITypeMembers _members;
		readonly IEnumeratorStore _enumerators;
		readonly IMemberAccessors _accessors;
		readonly Func<TypeInfo, string> _names;
		readonly Func<string, Namespace> _namespaces;

		[UsedImplicitly]
		public ObjectNamespaces(ITypeMembers members, IEnumeratorStore enumerators, IMemberAccessors accessors)
			: this(members, enumerators, accessors, Identities, new Namespaces().Get) {}

		public ObjectNamespaces(ITypeMembers members, IEnumeratorStore enumerators, IMemberAccessors accessors,
		                        Func<TypeInfo, string> names,
		                        Func<string, Namespace> namespaces)
		{
			_members = members;
			_enumerators = enumerators;
			_accessors = accessors;
			_names = names;
			_namespaces = namespaces;
		}

		public ImmutableArray<Namespace> Get(object parameter)
		{
			var identifiers = new ObjectTypeWalker(_members, _enumerators, _accessors, parameter)
				.Get()
				.Select(_names)
				.Distinct().ToArray();
			var result =
				new Namespace(Defaults.Xmlns.Name, identifiers[0]).Yield()
				                                                  .Concat(identifiers.Skip(1).Select(_namespaces))
				                                                  .ToImmutableArray();
			return result;
		}
	}
}