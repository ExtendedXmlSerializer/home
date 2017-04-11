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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class ObjectTypeWalker : InstanceMemberWalkerBase<TypeInfo>, ISource<IEnumerable<TypeInfo>>
	{
		readonly static VariableTypeMemberSpecifications Specifications = VariableTypeMemberSpecifications.Default;

		readonly IVariableTypeMemberSpecifications _specifications;
		readonly IMemberAccessors _accessors;

		public ObjectTypeWalker(ITypeMembers members, IEnumeratorStore enumerators, IMemberAccessors accessors, object root)
			: this(Specifications, accessors, members, enumerators, root) {}

		public ObjectTypeWalker(IVariableTypeMemberSpecifications specifications, IMemberAccessors accessors,
		                        ITypeMembers members, IEnumeratorStore enumerators, object root)
			: base(members, enumerators, root)
		{
			_specifications = specifications;
			_accessors = accessors;
		}

		protected override IEnumerable<TypeInfo> Yield(IMember member, object instance)
		{
			var variable = _specifications.Get(member);
			if (variable != null)
			{
				var current = _accessors.Get(member).Get(instance);
				if (Schedule(current) && variable.IsSatisfiedBy(current.GetType()))
				{
					yield return Defaults.FrameworkType;
				}
			}
		}

		protected override IEnumerable<TypeInfo> Yield(object instance)
		{
			Schedule(instance);
			yield break;
		}

		protected override IEnumerable<TypeInfo> Members(object input, TypeInfo parameter)
			=> parameter.Yield().Concat(base.Members(input, parameter));

		public IEnumerable<TypeInfo> Get() => this.SelectMany(x => x).Distinct();
	}
}