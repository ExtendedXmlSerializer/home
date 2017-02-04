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
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Xml.Properties;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	class GenericXmlElement : XmlElement
	{
		readonly ImmutableArray<IElement> _arguments;

		public GenericXmlElement(string displayName, TypeInfo classification, string ns, ImmutableArray<IElement> arguments)
			: base(displayName, classification, ns)
		{
			_arguments = arguments;
		}
	}

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

	sealed class VariableTypeEmitter : DecoratedEmitter
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
				writer.Attribute(TypeProperty.Default, actual);
			}
			base.Emit(writer, instance);
		}
	}

	class QualifiedNameTypeFormatter : /*CacheBase<TypeInfo, string>,*/ ITypeFormatter
	{
		readonly ITypeNames _names;

		public QualifiedNameTypeFormatter(ITypeNames names)
		{
			_names = names;
		}

		public string Get(TypeInfo parameter)
		{
			var name = _names.Get(parameter);
			var result = XmlQualifiedName.ToString(name.LocalName, name.NamespaceName);
			return result;
		}
	}

	public interface ITypeNames : IParameterizedSource<TypeInfo, XName> {}

	class TypeNames : /*CacheBase<TypeInfo, XName>,*/ ITypeNames
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
	}


	/*class XmlWriteContextFactory : IXmlWriteContextFactory
	{
		readonly IElements _elements;
		readonly INamespaces _namespaces;
		readonly XmlWriter _writer;
		readonly IDisposable _finish;
		readonly ImmutableArray<object> _services;

		public XmlWriteContextFactory(IElements elements, INamespaces namespaces, XmlWriter writer)
			: this(elements, namespaces, writer, writer) {}

		public XmlWriteContextFactory(IElements elements, INamespaces namespaces, XmlWriter writer, params object[] services)
			: this(elements, namespaces, writer, new DelegatedDisposable(writer.WriteEndElement), services.ToImmutableArray()) {}

		public XmlWriteContextFactory(IElements elements, INamespaces namespaces, XmlWriter writer, IDisposable finish,
		                              ImmutableArray<object> services)
		{
			_elements = elements;
			_namespaces = namespaces;
			_writer = writer;
			_finish = finish;
			_services = services;
		}

		public IWriteContext Create(IContainer container, TypeInfo instanceType)
		{
			return new XmlWriteContext(this, container, _elements.Load(container, instanceType));
		}

		public object GetService(Type serviceType)
		{
			var info = serviceType.GetTypeInfo();
			var length = _services.Length;
			for (var i = 0; i < length; i++)
			{
				var service = _services[i];
				if (info.IsInstanceOfType(service))
				{
					return service;
				}
			}
			return null;
		}

		public IDisposable Start(IWriteContext context)
		{
			var name = context.Container as IName ?? (context.Element as INamedElement)?.Name;
			if (name != null)
			{
				if (context.Container is IMember)
				{
					_writer.WriteStartElement(name.DisplayName);

					/*var type = instance.GetType().GetTypeInfo();
					if (!context.Container.Exact(type))
					{
						context.Write(TypeProperty.Default, type);
					}#1#
				}
				else
				{
					_writer.WriteStartElement(name.DisplayName, _namespaces.Get(name.Classification).Namespace.NamespaceName);
				}

				return _finish;
			}
			throw new SerializationException(
				$"Display name not found for element '{context.Element}' within a container of '{context.Container}.'");
		}

		public void Execute(string parameter) => _writer.WriteString(parameter);
	}*/
}