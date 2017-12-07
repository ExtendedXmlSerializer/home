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

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ConstructorMembers : CacheBase<ConstructorInfo, ImmutableArray<IMember>?>, IConstructorMembers
	{
		readonly IMetadataSpecification _specification;
		readonly IMembers _source;

		public ConstructorMembers(IMetadataSpecification specification, IMembers source)
		{
			_specification = specification;
			_source = source;
		}

		protected override ImmutableArray<IMember>? Create(ConstructorInfo parameter)
		{
			var parameters = parameter.GetParameters();
			if (parameters.Any())
			{
				var type = parameter.DeclaringType.GetTypeInfo();
				if (!IsGenericDictionarySpecification.Default.IsSatisfiedBy(type)) // A bit of a hack to circumvent https://github.com/wojtpl2/ExtendedXmlSerializer/issues/134
																				   // might want to pretty this up at some point.
				{
					var result = Yield(type, parameters).ToImmutableArray();
					if (result.Length == parameters.Length)
					{
						return result;
					}
				}
			}
			return null;
		}

		IEnumerable<IMember> Yield(TypeInfo type, ParameterInfo[] parameters)
		{
			var length = parameters.Length;
			for (var i = 0; i < length; i++)
			{
				var reflected = Members(type, parameters[i].Name).Only();
				var member = reflected != null ? _source.Get(reflected) : null;
				if (member != null && !member.IsWritable && Allowed(reflected))
				{
					yield return new ParameterizedMember(member);
				}
			}
		}

		bool Allowed(MemberInfo reflected)
		{
			switch (reflected)
			{
				case FieldInfo field:
					return _specification.IsSatisfiedBy(field);
				case PropertyInfo property:
					return _specification.IsSatisfiedBy(property);
				default:
					return false;
			}
		}

		static IEnumerable<MemberInfo> Members(TypeInfo typeInfo, string name)
		{
			foreach (var member in typeInfo.GetMembers().Where(IsSerializableMember.Default.IsSatisfiedBy))
			{
				if (member.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					yield return member;
				}
			}
		}
	}
}