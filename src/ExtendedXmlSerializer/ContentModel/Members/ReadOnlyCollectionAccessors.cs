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
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	sealed class ReadOnlyCollectionAccessors : OptionBase<IMember, IMemberAccess>
	{
		readonly IAllowedMemberValues _emit;
		readonly IGetterFactory _getter;
		readonly IAddDelegates _add;

		[UsedImplicitly]
		public ReadOnlyCollectionAccessors(IAllowedMemberValues emit)
			: this(emit, GetterFactory.Default, AddDelegates.Default) {}

		public ReadOnlyCollectionAccessors(IAllowedMemberValues emit, IGetterFactory getter, IAddDelegates add)
			: base(
				new MemberTypeSpecification(
					IsCollectionTypeSpecification.Default.And(
						new DelegatedAssignedSpecification<TypeInfo, Action<object, object>>(add.Get)))
			)
		{
			_emit = emit;
			_getter = getter;
			_add = add;
		}

		public override IMemberAccess Get(IMember parameter)
			=>
				new ReadOnlyCollectionMemberAccess(new MemberAccess(_emit.Get(parameter.Metadata), _getter.Get(parameter.Metadata),
				                                                    _add.Get(parameter.MemberType)));
	}
}