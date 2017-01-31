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

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.TypeModel;
using INames = ExtendedXmlSerialization.Conversion.INames;

namespace ExtendedXmlSerialization.ElementModel.Names
{
	public class NameProvider : NameProviderBase
	{
		public static NameProvider Default { get; } = new NameProvider();
		NameProvider() : this(TypeFormatter.Default) {}

		public NameProvider(ITypeFormatter formatter) : this(TypeAliasProvider.Default, formatter) {}

		public NameProvider(IAliasProvider alias, ITypeFormatter formatter) : base(alias, formatter) {}

		public override IName Create(string displayName, TypeInfo classification) => new Name(displayName, classification);
	}

	public class GenericNameProvider : NameProviderBase<IGenericName>
	{
		readonly INames _names;

		public GenericNameProvider(INames names) : this(names, TypeFormatter.Default) {}

		public GenericNameProvider(INames names, ITypeFormatter formatter)
			: this(names, TypeAliasProvider.Default, formatter) {}

		public GenericNameProvider(INames names, IAliasProvider alias, ITypeFormatter formatter)
			: base(alias, formatter)
		{
			_names = names;
		}

		public override IGenericName Create(string displayName, TypeInfo classification)
			=>
				new GenericName(displayName, classification,
				                classification.GetGenericArguments()
				                              .Select(x => x.GetTypeInfo())
				                              .Select(_names.Get)
				                              .ToImmutableArray());
	}
}