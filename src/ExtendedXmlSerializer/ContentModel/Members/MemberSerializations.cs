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

using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerializations : CacheBase<TypeInfo, IMemberSerialization>, IMemberSerializations
	{
		readonly Func<IEnumerable<IMemberSerializer>, IMemberSerialization> _builder;
		readonly ITypeMembers _members;
		readonly Func<IMember, IMemberSerializer> _serializers;

		[UsedImplicitly]
		public MemberSerializations(ITypeMembers members, IMemberSerializers serializers)
			: this(MemberSerializationBuilder.Default.Get, members, serializers.Get) {}

		public MemberSerializations(Func<IEnumerable<IMemberSerializer>, IMemberSerialization> builder, ITypeMembers members,
		                            Func<IMember, IMemberSerializer> serializers)
		{
			_builder = builder;
			_members = members;
			_serializers = serializers;
		}

		protected override IMemberSerialization Create(TypeInfo parameter)
		{
			var members = _members.Get(parameter)
			                      .Select(_serializers)
			                      .ToArray();
			var result = _builder(members);
			return result;
		}
	}
}