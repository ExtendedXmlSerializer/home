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
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypePartResolver : StructureCacheBase<TypeInfo, TypeParts>, ITypePartResolver
	{
		readonly IIdentities _identities;

		public TypePartResolver(IIdentities identities)
		{
			_identities = identities;
		}

		protected override TypeParts Create(TypeInfo parameter)
		{
			var identity = _identities.Get(parameter);
			var result = new TypeParts(identity.Name, identity.Identifier,
			                           parameter.IsGenericType ? Arguments(parameter.GetGenericArguments()) : null);
			return result;
		}

		Func<ImmutableArray<TypeParts>> Arguments(Type[] types)
		{
			var length = types.Length;
			var names = new TypeParts[length];
			for (var i = 0; i < length; i++)
			{
				names[i] = Get(types[i].GetTypeInfo());
			}
			var result = new Func<ImmutableArray<TypeParts>>(names.ToImmutableArray);
			return result;
		}
	}
}