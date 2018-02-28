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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class TypeIdentification : ITypeIdentification
	{
		readonly IParameterizedSource<TypeInfo, IIdentity> _identities;
		readonly IParameterizedSource<IIdentity, TypeInfo> _types;

		public TypeIdentification(IDictionary<TypeInfo, IIdentity> store)
			: this(new TableSource<TypeInfo, IIdentity>(store),
			       new TableSource<IIdentity, TypeInfo>(store.ToDictionary(x => x.Value, x => x.Key))) {}

		public TypeIdentification(IParameterizedSource<TypeInfo, IIdentity> identities,
		                          IParameterizedSource<IIdentity, TypeInfo> types)
		{
			_identities = identities;
			_types      = types;
		}

		public IIdentity Get(TypeInfo parameter) => _identities.Get(parameter);

		public TypeInfo Get(IIdentity parameter) => _types.Get(parameter);
	}
}