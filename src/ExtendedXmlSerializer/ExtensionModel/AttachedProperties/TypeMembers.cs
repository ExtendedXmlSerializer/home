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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	sealed class TypeMembers : ITypeMembers
	{
		readonly IIdentities _identities;
		readonly ITypeMembers _typeMembers;
		readonly IMembers _members;
		readonly ImmutableArray<IProperty> _properties;

		public TypeMembers(IIdentities identities, ITypeMembers typeMembers, IMembers members,
		                   ImmutableArray<IProperty> properties)
		{
			_identities = identities;
			_typeMembers = typeMembers;
			_members = members;
			_properties = properties;
		}

		public ImmutableArray<IMember> Get(TypeInfo parameter)
			=> _typeMembers.Get(parameter)
			               .AddRange(Yield(parameter).OrderBy(x => x.Order));

		IEnumerable<IMember> Yield(TypeInfo parameter)
		{
			foreach (var property in _properties)
			{
				if (property.IsSatisfiedBy(parameter))
				{
					var propertyInfo = property.Metadata;
					var identity = _identities.Get(propertyInfo.DeclaringType);
					yield return new AttachedMember(identity, _members.Get(propertyInfo), property);
				}
			}
		}
	}
}