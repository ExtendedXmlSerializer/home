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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Configuration
{
	class ExtendedXmlSerializerFactory : IExtendedXmlSerializerFactory
	{
		readonly static TypeSelector Selector = TypeSelector.Default;
		readonly static XmlFactory XmlFactory = XmlFactory.Default;

		public static ExtendedXmlSerializerFactory Default { get; } = new ExtendedXmlSerializerFactory();
		ExtendedXmlSerializerFactory() : this(Defaults.Property, Defaults.Field) {}

		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		public ExtendedXmlSerializerFactory(ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_property = property;
			_field = field;
		}

		public IExtendedXmlSerializer Get(IExtendedXmlConfiguration parameter)
		{
			var policy = parameter.Get();
			var serializers = new Serializers(_property.And(policy), _field.And(policy));
			var result = new ExtendedXmlSerializer(Selector, XmlFactory, serializers);
			return result;
		}
	}
}