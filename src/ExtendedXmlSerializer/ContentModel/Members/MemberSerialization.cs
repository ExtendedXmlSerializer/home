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

using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	sealed class MemberSerialization : CacheBase<MemberDescriptor, MemberProfile>, IMemberSerialization
	{
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;
		readonly IMemberEmitSpecifications _emit;
		readonly IMemberContent _content;
		readonly IMemberSerializers _serializers;
		readonly IAliases _aliases;
		readonly IMemberOrder _order;

		public MemberSerialization(
			ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field, IMemberEmitSpecifications emit,
			IMemberContent content, IMemberSerializers serializers, IAliases aliases, IMemberOrder order)
		{
			_property = property;
			_field = field;
			_emit = emit;
			_content = content;
			_serializers = serializers;
			_aliases = aliases;
			_order = order;
		}

		public bool IsSatisfiedBy(PropertyInfo parameter) => _property.IsSatisfiedBy(parameter);
		public bool IsSatisfiedBy(FieldInfo parameter) => _field.IsSatisfiedBy(parameter);

		protected override MemberProfile Create(MemberDescriptor parameter)
		{
			var metadata = parameter.Metadata;
			var order = _order.Get(metadata);
			var name = _aliases.Get(metadata) ?? metadata.Name;

			var content = _content.Get(parameter);

			var serializer = _serializers.Create(name, parameter, content);

			var result = new MemberProfile(_emit.Get(parameter), name, parameter.Writable, order, metadata,
			                               parameter.MemberType, content, serializer, serializer);
			return result;
		}
	}
}