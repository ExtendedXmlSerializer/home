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
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypePartResolver : StructureCacheBase<TypeInfo, TypeParts>, ITypePartResolver
	{
		readonly IIdentities _identities;
		readonly Func<TypeInfo, TypeInfo> _root;
		readonly Func<TypeInfo, ImmutableArray<int>> _dimensions;

		public TypePartResolver(IIdentities identities) : this(identities, RootType.Default.Get,
			ArrayTypeDimensions.Default.Get)
		{
		}

		public TypePartResolver(IIdentities identities, Func<TypeInfo, TypeInfo> root,
			Func<TypeInfo, ImmutableArray<int>> dimensions)
		{
			_identities = identities;
			_root = root;
			_dimensions = dimensions;
		}

		protected override TypeParts Create(TypeInfo parameter)
		{
			var array = parameter.IsArray;

			var identity = _identities.Get(array ? _root(parameter) : parameter);
			var arguments = parameter.IsGenericType ? Arguments(parameter.GetGenericArguments()) : null;
			var dimensions = array ? _dimensions(parameter) : (ImmutableArray<int>?) null;
			var result = new TypeParts(identity.Name, identity.Identifier, arguments, dimensions);
			return result;
		}

		Func<ImmutableArray<TypeParts>> Arguments(IReadOnlyList<Type> types)
		{
			var length = types.Count;
			var names = new TypeParts[length];
			for (var i = 0; i < length; i++)
			{
				names[i] = Get(types[i]
					.GetTypeInfo());
			}

			var result = new Func<ImmutableArray<TypeParts>>(names.ToImmutableArray);
			return result;
		}
	}
}