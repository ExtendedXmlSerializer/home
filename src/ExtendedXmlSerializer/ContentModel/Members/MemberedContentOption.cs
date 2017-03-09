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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	sealed class MemberedContentOption : ContentOptionBase
	{
		readonly static IsActivatedTypeSpecification Specification = IsActivatedTypeSpecification.Default;

		readonly IActivation _activation;
		readonly IMembers _members;

		public MemberedContentOption(IActivation activation, IMembers members)
			: base(Specification)
		{
			_activation = activation;
			_members = members;
		}

		public override ISerializer Get(TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var activator = _activation.Get(parameter);
			var reader = new MemberContentsReader(activator,
			                                      members.ToDictionary(x => x.Adapter.Name));

			var runtime = members.OfType<IRuntimeMember>().ToImmutableArray();
			var writer = runtime.Any() ? new RuntimeMemberListWriter(runtime, members) : (IWriter) new MemberListWriter(members);
			var result = new Serializer(reader, writer);
			return result;
		}
	}
}