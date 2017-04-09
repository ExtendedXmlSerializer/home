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
using ExtendedXmlSerializer.TypeModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ObjectIdentifiers : IObjectIdentifiers
	{
		readonly ITypeMembers _members;
		readonly IEnumeratorStore _enumerators;
		readonly IMemberAccessors _accessors;
		readonly Func<TypeInfo, string> _names;

		[UsedImplicitly]
		public ObjectIdentifiers(IIdentifiers identifiers, ITypeMembers members, IEnumeratorStore enumerators,
		                         IMemberAccessors accessors)
			: this(members, enumerators, accessors, identifiers.Get) {}

		ObjectIdentifiers(ITypeMembers members, IEnumeratorStore enumerators, IMemberAccessors accessors,
		                  Func<TypeInfo, string> names)
		{
			_members = members;
			_enumerators = enumerators;
			_accessors = accessors;
			_names = names;
		}

		public ImmutableArray<string> Get(object parameter)
			=> new ObjectTypeWalker(_members, _enumerators, _accessors, parameter).Get()
			                                                                      .Select(_names)
			                                                                      .Distinct()
			                                                                      .Skip(1)
			                                                                      .ToImmutableArray();
	}
}