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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Collections;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	public sealed class TypesInSameNamespace : Items<Type>
	{
		public TypesInSameNamespace(Type referenceType, IEnumerable<Type> candidates) :
			base(candidates.Where(x => x.Namespace == referenceType.Namespace)) {}
	}

	public sealed class PublicMembers : DelegatedSource<TypeInfo, IEnumerable<MemberInfo>>
	{
		public static PublicMembers Default { get; } = new PublicMembers();
		PublicMembers() : base(x => x.DeclaredMembers) {}
	}

	public sealed class Metadata : ItemsBase<MemberInfo>
	{
		readonly IEnumerable<TypeInfo> _types;
		readonly Func<TypeInfo, IEnumerable<MemberInfo>> _select;

		public Metadata(IEnumerable<Type> types) : this(types, PublicMembers.Default.Get) {}
		public Metadata(IEnumerable<Type> types, Func<TypeInfo, IEnumerable<MemberInfo>> select) : this(types.YieldMetadata(), @select) {}

		public Metadata(IEnumerable<TypeInfo> types) : this(types, PublicMembers.Default.Get) {}

		public Metadata(IEnumerable<TypeInfo> types, Func<TypeInfo, IEnumerable<MemberInfo>> select)
		{
			_types = types;
			_select = @select;
		}

		public override IEnumerator<MemberInfo> GetEnumerator() => _types.Concat(_types.SelectMany(_select))
		                                                                 .Distinct()
		                                                                 .GetEnumerator();
	}
}