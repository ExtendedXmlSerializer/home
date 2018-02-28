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

using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class TypeIdentityRegistrations : ITypeIdentityRegistrations
	{
		readonly Func<TypeInfo, IEnumerable<TypeInfo>> _types;
		readonly ITypeIdentity                         _identity;
		readonly IIdentityStore                        _store;
		readonly INamespaceFormatter                   _formatter;

		public TypeIdentityRegistrations(IDiscoveredTypes types, ITypeIdentity identity, IIdentityStore store,
		                                 INamespaceFormatter formatter)
			: this(types.To(EnumerableCoercer<TypeInfo>.Default)
			            .ToDelegate(), identity, store, formatter) {}

		TypeIdentityRegistrations(Func<TypeInfo, IEnumerable<TypeInfo>> types, ITypeIdentity identity, IIdentityStore store,
		                          INamespaceFormatter formatter)
		{
			_types     = types;
			_identity  = identity;
			_store     = store;
			_formatter = formatter;
		}

		public IEnumerable<KeyValuePair<TypeInfo, IIdentity>> Get(IEnumerable<TypeInfo> parameter)
		{
			foreach (var type in parameter.SelectMany(_types)
			                              .Distinct())
			{
				var key = _identity.Get(type);
				if (key != null)
				{
					var identifier = key.Value.Identifier.NullIfEmpty() ?? _formatter.Get(type);
					var name = key.Value.Name.NullIfEmpty() ?? type.Name;
					var identity = _store.Get(name, identifier);
					yield return Pairs.Create(type, identity);
				}
			}
		}
	}
}