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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	sealed class Identities : CacheBase<TypeInfo, IIdentity>, IIdentities
	{
		readonly static TypeNameFormatter TypeNameFormatter = TypeNameFormatter.Default;

		readonly IIdentityStore _source;
		readonly INames _alias;
		readonly ITypeFormatter _formatter;
		readonly IIdentifiers _identifiers;

		[UsedImplicitly]
		public Identities(IIdentifiers identifiers, IIdentityStore source, INames names)
			: this(source, names, TypeNameFormatter, identifiers) {}

		public Identities(IIdentityStore source, INames alias, ITypeFormatter formatter, IIdentifiers identifiers)
		{
			_source = source;
			_alias = alias;
			_formatter = formatter;
			_identifiers = identifiers;
		}

		protected override IIdentity Create(TypeInfo parameter)
			=> _source.Get(_alias.Get(parameter) ?? _formatter.Get(parameter), _identifiers.Get(parameter));
	}
}