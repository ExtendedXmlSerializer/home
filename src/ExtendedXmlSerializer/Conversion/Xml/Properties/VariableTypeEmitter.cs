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
using System.Collections.Immutable;
using System.Reflection;
using System.Xml;

namespace ExtendedXmlSerialization.Conversion.Xml.Properties
{
	public interface ITypeProperty : IProperty<TypeInfo> {}

	sealed class TypeProperty : FrameworkElementBase<TypeInfo>, ITypeProperty
	{
		public static TypeProperty Default { get; } = new TypeProperty();
		TypeProperty() : this(TypeNames.Default) {}

		readonly ITypeNames _names;
		
		public TypeProperty(ITypeNames names) : base("type")
		{
			_names = names;
		}

		public override void Emit(XmlWriter writer, TypeInfo instance)
		{
			var property = _names.Format(writer, instance);
			writer.WriteAttributeString(Name.LocalName, Name.NamespaceName, property);
		}
	}

	public interface ITypeArgumentsProperty : IProperty<ImmutableArray<Type>> {}
	sealed class TypeArgumentsProperty : FrameworkElementBase<ImmutableArray<Type>>, ITypeArgumentsProperty
	{
		public static TypeArgumentsProperty Default { get; } = new TypeArgumentsProperty();
		TypeArgumentsProperty() : this(TypeNames.Default) {}

		readonly ITypeNames _names;

		public TypeArgumentsProperty(ITypeNames names) : base("arguments")
		{
			_names = names;
		}

		public override void Emit(XmlWriter writer, ImmutableArray<Type> instance)
		{
			var value = _names.FormatArguments(writer, instance);
			writer.WriteAttributeString(Name.LocalName, Name.NamespaceName, value);
		}
	}
}