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

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;
using Member = ExtendedXmlSerialization.ContentModel.Content.Member;

namespace ExtendedXmlSerialization.ContentModel
{
	class SerializationProfile : ISerializationProfile
	{
		readonly ISerialization _serialization;
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;
		readonly Members.IAliases _aliases;
		readonly IMemberOrder _order;
		readonly IReadOnlyList<IContainerOption> _options;

		public SerializationProfile(
			ISerialization serialization,
			ISpecification<PropertyInfo> property,
			ISpecification<FieldInfo> field,
			Members.IAliases aliases,
			IMemberOrder order,
			IReadOnlyList<IContainerOption> options)
		{
			_serialization = serialization;
			_property = property;
			_field = field;
			_aliases = aliases;
			_order = order;
			_options = options;
		}

		public IMemberProfile Get(IProperty parameter)
		{
			var propertyInfo = parameter.Get();
			return _property.IsSatisfiedBy(propertyInfo)
				? Create(propertyInfo, parameter.MemberType, propertyInfo.CanWrite)
				: null;
		}

		public IMemberProfile Get(IField parameter)
		{
			var fieldInfo = parameter.Get();
			return _field.IsSatisfiedBy(fieldInfo)
				? Create(fieldInfo, parameter.MemberType, !fieldInfo.IsInitOnly)
				: null;
		}

		IMemberProfile Create(MemberInfo metadata, TypeInfo memberType, bool assignable)
		{
			var name = _aliases.Get(metadata) ?? metadata.Name;
			var order = _order.Get(metadata);
			var element = new Member(name);
			var type = memberType.AccountForNullable();
			var content = _serialization.Get(type).Get();
			var result = new MemberProfile(
				AlwaysSpecification<object>.Default,
				name, assignable, order, metadata, type, element, content);
			return result;
		}

		public IEnumerator<IContainerOption> GetEnumerator() => _options.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}