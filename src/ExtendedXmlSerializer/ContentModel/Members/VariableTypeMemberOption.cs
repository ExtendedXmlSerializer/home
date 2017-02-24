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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class VariableTypeMemberOption : MemberOption
	{
		readonly static VariableTypeMemberSpecification Specification = VariableTypeMemberSpecification.Default;
		readonly ISerializer _runtime;

		public VariableTypeMemberOption(ISerializer runtime) : base(Specification)
		{
			_runtime = runtime;
		}

		protected sealed override IMember CreateMember(MemberProfile profile, Func<object, object> getter, Action<object, object> setter)
		{
			// TODO: This should be simplified:
			var specification = new EqualitySpecification<Type>(profile.MemberType.AsType()).Inverse();
			var writer = new Enclosure(new MemberElement(profile.Identity.Name),
			                           new VariableTypedMemberWriter(specification, _runtime, profile.Content));
			var decorated = new MemberProfile(profile.Specification, profile.Identity.Name, profile.AllowWrite, profile.Order,
			                                  profile.Metadata, profile.MemberType, profile.Content, profile.Reader, writer);
			var adapter = new VariableTypeMemberAdapter(specification, new MemberAdapterSelector(getter, setter).Get(decorated));
			var result = base.CreateMember(decorated, adapter);
			return result;
		}
	}
}