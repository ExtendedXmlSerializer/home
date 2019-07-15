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

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceWalker : InstanceMemberWalkerBase<object>, ISource<ImmutableArray<object>>
	{
		readonly IReferencesPolicy _policy;
		readonly IMemberAccessors  _accessors;

		public ReferenceWalker(IReferencesPolicy policy, ITypeMembers members, IEnumeratorStore enumerators,
		                       IMemberAccessors accessors, object root)
			: base(members, enumerators, root)
		{
			_policy    = policy;
			_accessors = accessors;
		}

		protected override IEnumerable<object> Yield(IMember member, object instance)
			=> Yield(_accessors.Get(member)
			                   .Get(instance));

		protected override IEnumerable<object> Yield(object instance)
		{
			if (!Schedule(instance) && Check(instance))
			{
				yield return instance;
			}
		}

		bool Check(object instance)
		{
			var info = instance?.GetType()
			                   .GetTypeInfo();
			var check = info != null && !info.IsValueType && _policy.IsSatisfiedBy(info);
			return check;
		}

		public ImmutableArray<object> Get() => this.SelectMany(x => x)
		                                           .Distinct()
		                                           .ToImmutableArray();
	}
}