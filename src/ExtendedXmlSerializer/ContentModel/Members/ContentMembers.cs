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

using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class ContentMembers : CacheBase<TypeInfo, ImmutableArray<IMember>>, IContentMembers
	{
		readonly Func<IMember, bool> _specification;
		readonly ITypeMembers _source;

		[UsedImplicitly]
		public ContentMembers(IValidMemberSpecification specification, ITypeMembers source)
			: this(specification.IsSatisfiedBy, source) {}

		public ContentMembers(Func<IMember, bool> specification, ITypeMembers source)
		{
			_specification = specification;
			_source = source;
		}

		protected override ImmutableArray<IMember> Create(TypeInfo parameter)
		{
			var result = _source.Get(parameter)
			                    .Where(_specification)
			                    .OrderBy(x => x.Order)
			                    .ToImmutableArray();
			return result;
		}
	}

	/*sealed class ContentMembers<T> : IContentMembers<T>
	{
		readonly Func<IMember, bool> _specification;
		readonly Func<IEnumerable<IMember>> _members;

		[UsedImplicitly]
		public ContentMembers(IValidMemberSpecification specification, ITypeMembers source)
			: this(specification.IsSatisfiedBy, source.Fix(Support<T>.Key).ToDelegate()) { }

		public ContentMembers(Func<IMember, bool> specification, Func<IEnumerable<IMember>> members)
		{
			_specification = specification;
			_members = members;
		}

		public ImmutableArray<IMember> Get() => _members.Invoke()
		                                                .Where(_specification)
		                                                .OrderBy(x => x.Order)
		                                                .ToImmutableArray();
	}*/

}