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
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Properties;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	class XmlMemberAdorner : XmlMemberAdornerBase
	{
		public static XmlMemberAdorner Default { get; } = new XmlMemberAdorner();
		XmlMemberAdorner() : this(XmlMemberWritable.Default.Accept) {}

		readonly Func<IElement, IXmlWritable> _writable;

		XmlMemberAdorner(Func<IElement, IXmlWritable> writable)
		{
			_writable = writable;
		}

		protected override IXmlWritable GetWritable(IElement parameter) => _writable(parameter);
	}

	abstract class XmlMemberAdornerBase : IMemberAdorner
	{
		protected abstract IXmlWritable GetWritable(IElement parameter);

		public IElement Get(IElement parameter) => new XmlElement(GetWritable(parameter), parameter);
	}

	class XmlMemberWritable : IXmlWritable
	{
		public static XmlMemberWritable Default { get; } = new XmlMemberWritable();
		XmlMemberWritable() {}

		public void Write(System.Xml.XmlWriter writer, IElement element, object instance)
			=> writer.WriteStartElement(element.DisplayName);
	}

	class XmlVariableMemberAdorner : XmlMemberAdornerBase
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IXmlWritable _variable, _fixed;

		public XmlVariableMemberAdorner(ITypeNames names, INames dataNames)
			: this(
				FixedTypeSpecification.Default, new XmlVariableTypeMemberWritable(names, dataNames.Get(TypeProperty.Default)),
				XmlMemberWritable.Default) {}

		public XmlVariableMemberAdorner(ISpecification<TypeInfo> specification, IXmlWritable variable, IXmlWritable @fixed)
		{
			_specification = specification;
			_variable = variable;
			_fixed = @fixed;
		}

		protected override IXmlWritable GetWritable(IElement parameter) 
			=> _specification.IsSatisfiedBy(parameter.Classification) ? _fixed : _variable;
	}

	class XmlElement : DecoratedElement, IXmlElement
	{
		readonly IXmlWritable _writable;
		readonly IElement _element;

		public XmlElement(IXmlWritable writable, IElement element) : base(element)
		{
			_writable = writable;
			_element = element;
		}

		public void Write(System.Xml.XmlWriter writer, object instance) => _writable.Write(writer, _element, instance);
	}


	class XmlVariableTypeMemberWritable : IXmlWritable
	{
		readonly ITypeNames _names;
		readonly XName _type;
		readonly IXmlWritable _writable;

		public XmlVariableTypeMemberWritable(ITypeNames names, XName type) : this(names, type, XmlMemberWritable.Default) {}

		public XmlVariableTypeMemberWritable(ITypeNames names, XName type, IXmlWritable writable)
		{
			_names = names;
			_type = type;
			_writable = writable;
		}

		public void Write(System.Xml.XmlWriter writer, IElement element, object instance)
		{
			_writable.Write(writer, element, instance);

			var classification = instance.GetType().GetTypeInfo();
			if (!element.Exact(classification))
			{
				var native = _names.Get(classification);
				writer.WriteStartAttribute(_type.LocalName, _type.NamespaceName);
				writer.WriteQualifiedName(native.LocalName, native.NamespaceName);
				writer.WriteEndAttribute();
			}
		}
	}

	class QualifiedNameTypeFormatter : CacheBase<TypeInfo, string>, ITypeFormatter
	{
		readonly ITypeNames _names;

		public QualifiedNameTypeFormatter(ITypeNames names)
		{
			_names = names;
		}

		protected override string Create(TypeInfo parameter)
		{
			var name = _names.Get(parameter);
			var result = XmlQualifiedName.ToString(name.LocalName, name.NamespaceName);
			return result;
		}
	}

	public interface ITypeNames : IParameterizedSource<TypeInfo, XName> {}

	class TypeNames : CacheBase<TypeInfo, XName>, ITypeNames
	{
		readonly IElements _elements;
		readonly INames _native;

		public TypeNames(IElements elements, INames native)
		{
			_elements = elements;
			_native = native;
		}

		protected override XName Create(TypeInfo parameter) => _native.Get(_elements.Get(parameter));
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