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
using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion.Xml.Properties
{
	sealed class VariableTypeEmitter : FrameworkElementBase, IRenderXml
	{
		readonly IElement _element;
		readonly ITypeNames _names;

		public VariableTypeEmitter(IElement element) : this(element, TypeNames.Default) {}

		public VariableTypeEmitter(IElement element, ITypeNames names) : base("type")
		{
			_element = element;
			_names = names;
		}

		public void Render(System.Xml.XmlWriter writer, object instance)
		{
			var classification = instance.GetType().GetTypeInfo();
			if (!_element.Exact(classification))
			{
				var native = _names.Get(classification);
				writer.WriteStartAttribute(DisplayName, Namespace);
				writer.WriteQualifiedName(native.LocalName, native.NamespaceName);
				writer.WriteEndAttribute();
			}
		}
	}

	sealed class TypeArgumentsProperty : FrameworkElementBase
	{
		public static TypeArgumentsProperty Default { get; } = new TypeArgumentsProperty();
		TypeArgumentsProperty() : base("arguments") {}
	}
}