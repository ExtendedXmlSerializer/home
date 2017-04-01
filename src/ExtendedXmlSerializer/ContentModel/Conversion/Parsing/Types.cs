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
using ExtendedXmlSerializer.ContentModel.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion.Parsing
{
	sealed class Types : ITypes
	{
		readonly static IdentityStore IdentityStore = IdentityStore.Default;

		readonly IIdentityStore _identities;
		readonly IGenericTypes _types;
		readonly IXmlReader _reader;

		public Types(IGenericTypes genericTypes, IXmlReader reader) : this(IdentityStore, genericTypes, reader) {}

		public Types(IIdentityStore identities, IGenericTypes types, IXmlReader reader)
		{
			_identities = identities;
			_types = types;
			_reader = reader;
		}

		public TypeInfo Get(TypeParts parts)
		{
			var identity = _identities.Get(parts.Name, _reader.Get(parts.Identifier));
			var typeInfo = _types.Get(identity);
			var arguments = parts.GetArguments();
			var result = arguments.HasValue ? typeInfo.MakeGenericType(Arguments(arguments.Value)).GetTypeInfo() : typeInfo;
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