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

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class TypeMemberSource : ITypeMemberSource
	{
		readonly IMetadataSpecification _specification;
		readonly IMembers _profiles;

		public TypeMemberSource(IMetadataSpecification specification, IMembers profiles)
		{
			_specification = specification;
			_profiles = profiles;
		}

		public IEnumerable<IMember> Get(TypeInfo parameter)
		{
			var alteration = new ReflectedTypeAlteration(parameter);
			var properties = parameter.GetProperties().Select(alteration.Get).ToArray();
			var length = properties.Length;
			for (var i = 0; i < length; i++)
			{
				var property = properties[i];
				if (_specification.IsSatisfiedBy(property))
				{
					yield return _profiles.Get(property);
				}
			}

			var fields = parameter.GetFields().Select(alteration.Get).ToArray();
			var l = fields.Length;
			for (var i = 0; i < l; i++)
			{
				var field = fields[i];
				if (_specification.IsSatisfiedBy(field))
				{
					yield return _profiles.Get(field);
				}
			}
		}
	}
}