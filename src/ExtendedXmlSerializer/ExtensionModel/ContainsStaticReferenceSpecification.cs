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

using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel
{
	class ContainsStaticReferenceSpecification : DelegatedSpecification<TypeInfo>, IStaticReferenceSpecification
	{
		public ContainsStaticReferenceSpecification(ITypeMembers members) : base(new Cache(members).Get) {}

		sealed class Cache : CacheBase<TypeInfo, bool>
		{
			readonly ITypeMembers _members;

			public Cache(ITypeMembers members)
			{
				_members = members;
			}

			protected override bool Create(TypeInfo parameter)
			{
				var variables = new VariableTypeWalker(_members, parameter).Get().ToArray();
				var length = variables.Length;
				for (var i = 0; i < length; i++)
				{
					var first = variables[i];
					for (var j = 0; j < length; j++)
					{
						var second = variables[j];
						if (i != j &&
						    (first.IsInterface || second.IsInterface || first.IsAssignableFrom(second) || second.IsAssignableFrom(first))
						)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
	}
}