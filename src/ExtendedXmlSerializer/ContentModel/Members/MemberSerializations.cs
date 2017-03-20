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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.TypeModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerializations : ReferenceCacheBase<TypeInfo, IMemberSerialization>, IMemberSerializations
	{
		readonly static Func<IMemberSerializer, bool> Property =
			IsTypeSpecification<PropertyMemberSerializer>.Default.IsSatisfiedBy;

		readonly Func<IMemberSerializer, bool> _property;
		readonly ITypeMembers _profiles;
		readonly Func<IMember, IMemberSerializer> _serializers;

		[UsedImplicitly]
		public MemberSerializations(ITypeMembers profiles, IMemberSerializers serializers)
			: this(Property, profiles, serializers.Get) {}

		public MemberSerializations(Func<IMemberSerializer, bool> property, ITypeMembers profiles,
		                            Func<IMember, IMemberSerializer> serializers)
		{
			_property = property;
			_profiles = profiles;
			_serializers = serializers;
		}

		protected override IMemberSerialization Create(TypeInfo parameter)
		{
			var serializers = _profiles.Get(parameter).Select(_serializers).ToArray();
			var properties = serializers.Where(_property).ToArray();
			var runtime = serializers.OfType<IRuntimeSerializer>().ToArray();
			var contents = serializers.Except(properties.Concat(runtime)).ToArray();
			var list = runtime.Any()
				? new RuntimeMemberList(_property, properties, runtime, contents)
				: (IRuntimeMemberList) new FixedRuntimeMemberList(properties.Concat(contents).ToImmutableArray());
			var all = properties.Concat(runtime).Concat(contents).OrderBy(x => x.Profile.Order).ToImmutableArray();
			var result = new MemberSerialization(list, serializers.ToDictionary(x => x.Profile.Name, x => x), all);
			return result;
		}
	}
}