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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class Serialization : ISerialization
	{
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		readonly Func<TypeInfo, IContainer> _selector;

		public Serialization(ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
			: this(property, field, new ContainerOptions().Get) {}

		public Serialization(ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field,
		                   Func<ISerialization, IEnumerable<IContainerOption>> options)
		{
			_property = property;
			_field = field;

			_selector = new Selector<TypeInfo, IContainer>(options(this).ToArray()).Cache().Get;
		}

		public IContainer Get(TypeInfo parameter) => _selector(parameter);
		
		public bool IsSatisfiedBy(PropertyInfo parameter) => _property.IsSatisfiedBy(parameter);

		public bool IsSatisfiedBy(FieldInfo parameter) => _field.IsSatisfiedBy(parameter);
	}
}