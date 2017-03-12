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
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class VariableTypeWalker : TypeMemberWalkerBase<TypeInfo>, ISource<IEnumerable<TypeInfo>>
	{
		readonly static VariableTypeSpecification Specification = VariableTypeSpecification.Default;

		readonly ISpecification<TypeInfo> _specification;

		public VariableTypeWalker(ITypeMembers members, TypeInfo root) : this(Specification, members, root) {}

		public VariableTypeWalker(ISpecification<TypeInfo> specification, ITypeMembers members, TypeInfo root)
			: base(members, root)
		{
			_specification = specification;
		}

		protected override IEnumerable<TypeInfo> Select(TypeInfo type)
		{
			foreach (var typeInfo in type.Yield().Concat(base.Select(type)))
			{
				if (_specification.IsSatisfiedBy(typeInfo))
				{
					yield return typeInfo;
				}
			}
		}

		protected override IEnumerable<TypeInfo> Yield(IMember member)
		{
			var type = member.MemberType;
			if (!Schedule(type))
			{
				yield return type;
			}
		}

		public IEnumerable<TypeInfo> Get() => this.SelectMany(x => x);
	}
}