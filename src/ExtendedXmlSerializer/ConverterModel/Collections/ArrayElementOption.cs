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
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.ConverterModel.Properties;
using ExtendedXmlSerialization.ConverterModel.Xml;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Collections
{
	class ArrayElementOption : ElementOptionBase
	{
		public static ArrayElementOption Default { get; } = new ArrayElementOption();
		ArrayElementOption() : this(CollectionItemTypeLocator.Default) {}

		readonly ICollectionItemTypeLocator _locator;

		public ArrayElementOption(ICollectionItemTypeLocator locator) : base(IsArraySpecification.Default)
		{
			_locator = locator;
		}

		public override IWriter Get(TypeInfo parameter) => new ArrayElement(_locator.Get(parameter));
	}

	class ArrayElement : Element
	{
		readonly static XName XName = Names.Default.Get(typeof(Array).GetTypeInfo());

		readonly TypeInfo _elementType;
		readonly ITypeProperty _property;

		public ArrayElement(TypeInfo element) : this(XName, element, ItemTypeProperty.Default) {}

		public ArrayElement(XName name, TypeInfo elementType, ITypeProperty property) : base(name)
		{
			_elementType = elementType;
			_property = property;
		}

		public override void Write(IXmlWriter writer, object instance)
		{
			base.Write(writer, instance);
			_property.Write(writer, _elementType);
		}
	}
}