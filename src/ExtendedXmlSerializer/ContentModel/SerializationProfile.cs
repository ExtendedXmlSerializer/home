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
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel
{
	class SerializationProfile : ISerializationProfile
	{
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;
		readonly IMemberEmitSpecifications _emit;
		readonly IMemberSerializers _serializers;
		readonly ISerialization _serialization;
		readonly Members.IAliases _aliases;
		readonly IMemberOrder _order;
		readonly IReadOnlyList<IContainerOption> _options;

		public SerializationProfile(
			ISpecification<PropertyInfo> property,
			ISpecification<FieldInfo> field,
			IMemberEmitSpecifications emit,
			ISerialization serialization,
			IMemberSerializers serializers,
			Members.IAliases aliases,
			IMemberOrder order,
			IReadOnlyList<IContainerOption> options)
		{
			_property = property;
			_field = field;
			_emit = emit;
			_serializers = serializers;
			_serialization = serialization;
			_aliases = aliases;
			_order = order;
			_options = options;
		}

		public bool IsSatisfiedBy(PropertyInfo parameter) => _property.IsSatisfiedBy(parameter);
		public bool IsSatisfiedBy(FieldInfo parameter) => _field.IsSatisfiedBy(parameter);

		public MemberProfile Get(MemberDescriptor parameter)
		{
			var metadata = parameter.Metadata;
			var order = _order.Get(metadata);
			var name = _aliases.Get(metadata) ?? metadata.Name;

			var content = _serialization.Get(parameter.MemberType).Get();

			var writer = _serializers.Create(name, parameter, content);

			var result = new MemberProfile(_emit.Get(parameter), name, parameter.Writable, order, metadata,
			                               parameter.MemberType, content, writer);
			return result;
		}

		public IEnumerator<IContainerOption> GetEnumerator() => _options.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}