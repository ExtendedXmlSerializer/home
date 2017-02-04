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

namespace ExtendedXmlSerialization.Conversion.Xml
{
	/*class GenericXmlElement : GenericElement, IXmlElement
	{
		public GenericXmlElement(string displayName, TypeInfo classification, string ns, ImmutableArray<IElement> arguments)
			: base(displayName, classification, arguments)
		{
			Namespace = ns;
		}

		public string Namespace { get; }
	}
	*/
	class XmlElement : IXmlElement
	{
		public XmlElement(string displayName, Type type, string ns) : this(displayName, type.GetTypeInfo(), ns) {}

		public XmlElement(string displayName, TypeInfo classification, string ns)
		{
			DisplayName = displayName;
			Classification = classification;
			Namespace = ns;
		}

		public string DisplayName { get; }
		public TypeInfo Classification { get; }
		public string Namespace { get; }
	}

	/*sealed class VariableTypeEmitter : DecoratedEmitter
	{
		readonly IElement _element;
		readonly IElements _elements;

		public VariableTypeEmitter(IElement element, IEmitter emitter) : this(element, Elements.Default, emitter) {}

		public VariableTypeEmitter(IElement element, IElements elements, IEmitter emitter) : base(emitter)
		{
			_element = element;
			_elements = elements;
		}

		public override void Emit(IWriter writer, object instance)
		{
			var actual = _elements.Actual(_element, instance);
			if (actual != null)
			{
				var xml = (IXmlWriter) writer;

				writer.Attribute(TypeProperty.Default, actual);
			}
			base.Emit(writer, instance);
		}
	}*/

	/*class QualifiedNameTypeFormatter : /*CacheBase<TypeInfo, string>,#1# ITypeFormatter
	{
		readonly XmlWriter _writer;
		readonly ITypeNames _names;

		public QualifiedNameTypeFormatter(XmlWriter writer) : this(writer, TypeNames.Default) {}

		public QualifiedNameTypeFormatter(XmlWriter writer, ITypeNames names)
		{
			_writer = writer;
			_names = names;
		}

		public string Get(TypeInfo parameter)
		{
			var name = _names.Get(parameter);
			var type = XmlQualifiedName.ToString(name.LocalName, _writer.GetPrefix(name.NamespaceName));
			var result = parameter.IsGenericType ? parameter.GetGenericArguments()
			return type;
		}
	}*/

	/*public interface ITypeNames : IParameterizedSource<TypeInfo, XName> {}

	class TypeNames : /*CacheBase<TypeInfo, XName>,#1# ITypeNames
	{
		public static TypeNames Default { get; } = new TypeNames();
		TypeNames() : this(Elements.Default) {}

		readonly IElements _elements;
		readonly INames _native;

		public TypeNames(IElements elements) : this(elements, Names.Default) {}

		public TypeNames(IElements elements, INames native)
		{
			_elements = elements;
			_native = native;
		}

		public XName Get(TypeInfo parameter) => _native.Get(_elements.Get(parameter));
	}*/
}