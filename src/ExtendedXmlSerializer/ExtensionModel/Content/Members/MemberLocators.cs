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

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class MemberLocators : ReferenceCacheBase<TypeInfo, IMetadataLocator>, IMemberLocators
	{
		public static MemberLocators Default { get; } = new MemberLocators();

		MemberLocators() : this(IsSerializableMember.Default.IsSatisfiedBy) {}

		readonly Func<MemberInfo, bool> _specification;

		public MemberLocators(Func<MemberInfo, bool> specification) => _specification = specification;

		protected override IMetadataLocator Create(TypeInfo parameter)
			=> new MetadataLocator(parameter.GetMembers()
			                                .GroupBy(x => x.Name)
			                                .Where(x => x.Count() == 1)
			                                .SelectMany(x => x.ToArray())
			                                .Where(_specification)
			                                .ToImmutableArray());
	}
}