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

using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypePartReflector : CacheBase<TypeParts, TypeInfo>, ITypePartReflector
	{
		readonly IIdentityStore _identities;
		readonly ITypes _types;

		public TypePartReflector(IIdentityStore identities, ITypes types)
		{
			_identities = identities;
			_types = types;
		}

		protected override TypeInfo Create(TypeParts parameter)
		{
			var identity = _identities.Get(parameter.Name, parameter.Identifier);
			var typeInfo = _types.Get(identity);
			var arguments = parameter.GetArguments();
			var result = arguments.HasValue ? typeInfo.MakeGenericType(Arguments(arguments.Value)).GetTypeInfo() : typeInfo;
			if (result == null)
			{
				throw new ParseException(
					$"An attempt was made to parse the identity '{IdentityFormatter.Default.Get(identity)}', but no type could be located with that name.");
			}
			return result;
		}

		Type[] Arguments(ImmutableArray<TypeParts> names)
		{
			var length = names.Length;
			var result = new Type[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = Get(names[i]).AsType();
			}
			return result;
		}
	}
}