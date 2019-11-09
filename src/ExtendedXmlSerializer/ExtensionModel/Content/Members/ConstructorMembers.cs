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
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ConstructorMembers : CacheBase<ConstructorInfo, ImmutableArray<IMember>?>, IConstructorMembers
	{
		readonly IMembers            _members;
		readonly IMemberLocators     _locator;
		readonly Func<IMember, bool> _member;

		public ConstructorMembers(IMembers members, IMemberLocators locator, IMemberSpecification member)
			: this(members, locator, member.IsSatisfiedBy) {}

		ConstructorMembers(IMembers members, IMemberLocators locator, Func<IMember, bool> member)
		{
			_members = members;
			_locator = locator;
			_member  = member;
		}

		protected override ImmutableArray<IMember>? Create(ConstructorInfo parameter)
		{
			var type = parameter.DeclaringType.GetTypeInfo();
			if (!IsGenericDictionarySpecification.Default.IsSatisfiedBy(type))
				// A bit of a hack to circumvent https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/134
				// might want to pretty this up at some point.
			{
				var source = parameter.GetParameters();
				if (source.Length > 0 || IsCollectionTypeSpecification.Default.IsSatisfiedBy(type)) // Turning into a game of Jenga here.
				{
					var result = source.Select(x => x.Name)
					                   .Select(_locator.Get(type)
					                                   .Get)
					                   .Where(x => x.HasValue)
					                   .Select(x => x.Value)
					                   .Select(_members.Get)
					                   .Where(_member)
					                   .Select(x => new ParameterizedMember(x))
					                   .ToImmutableArray<IMember>();
					if (result.Length == source.Length)
					{
						return result;
					}
				}
			}

			return null;
		}
	}
}