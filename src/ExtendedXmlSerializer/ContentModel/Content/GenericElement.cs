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
using ExtendedXmlSerialization.ContentModel.Properties;
using ExtendedXmlSerialization.ContentModel.Xml;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class GenericElement : Element
	{
		readonly TypeInfo _genericType;
		readonly IQualifiedNameArgumentsProperty _property;

		public GenericElement(XName name, TypeInfo genericType) : this(name, genericType, TypeArgumentsProperty.Default) {}

		public GenericElement(XName name, TypeInfo genericType, IQualifiedNameArgumentsProperty property) : base(name)
		{
			_genericType = genericType;
			_property = property;
		}

		public override void Write(IXmlWriter writer, object instance)
		{
			base.Write(writer, instance);
			var names = writer.Get(_genericType).GetArguments();
			if (names.HasValue)
			{
				writer.Property(_property, names.Value);
			}
			else
			{
				throw new InvalidOperationException($"The generic type '{_genericType}' contains generic type arguments, but they could not be parsed.");
			}
		}
	}

/*
	abstract class TypedElement : Element
	{
		readonly TypeInfo _type;
		readonly IQualifiedNameProperty _property;

		protected TypedElement(XName name, TypeInfo type, IQualifiedNameProperty property) : base(name)
		{
			_type = type;
			_property = property;
		}

		public override void Write(IXmlWriter writer, object instance)
		{
			base.Write(writer, instance);
			writer.Property(_property, writer.Get(_type));
		}
	}
*/
}