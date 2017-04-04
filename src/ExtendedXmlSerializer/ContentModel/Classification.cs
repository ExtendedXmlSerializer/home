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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Xml;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class Classification : IClassification
	{
		readonly static IdentityStore IdentityStore = IdentityStore.Default;

		readonly IIdentityStore _identities;
		readonly ITypes _types;

		public Classification(ITypes types) : this(IdentityStore, types) {}

		public Classification(IIdentityStore identities, ITypes types)
		{
			_identities = identities;
			_types = types;
		}

		public TypeInfo Get(IContentAdapter parameter) => FromAttributes(parameter) ?? FromIdentity(parameter);

		static TypeInfo FromAttributes(IContentAdapter parameter)
			=> parameter.Any()
				? ExplicitTypeProperty.Default.Get(parameter) ?? ItemTypeProperty.Default.Get(parameter)
				: null;

		TypeInfo FromIdentity(IContentAdapter parameter)
		{
			var identity = _identities.Get(parameter.Name, parameter.Identifier);

			var result = _types.Get(identity);

			var arguments = ArgumentsTypeProperty.Default.Get(parameter);
			if (arguments.HasValue)
			{
				return result.MakeGenericType(arguments.Value.ToArray()).GetTypeInfo();
			}

			if (result.IsGenericTypeDefinition)
			{
				throw new InvalidOperationException(
					"An attempt was made to create a generic type, but no type arguments were found.");
			}

			return result;
		}
	}
}