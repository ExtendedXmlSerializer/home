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
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	class XmlWriteContext : IWriteContext
	{
		readonly IElements _selector;
		readonly INamespaces _namespaces;
		readonly XmlWriter _writer;
		readonly ImmutableArray<object> _services;
		readonly IDisposable _finish;

		public XmlWriteContext(XmlWriter writer, IRoot root) : this(writer, root, writer) {}

		public XmlWriteContext(XmlWriter writer, IRoot root, params object[] services)
			: this(Elements.Default, Namespaces.Default, writer, root, services) {}

		public XmlWriteContext(IElements selector, INamespaces namespaces, XmlWriter writer, IRoot root,
		                       params object[] services)
			: this(
				selector, namespaces, root, selector.Get(root.Classification), writer, services.ToImmutableArray(),
				new DelegatedDisposable(writer.WriteEndElement)) {}

		XmlWriteContext(IElements selector, INamespaces namespaces,
		                IContainerElement container, IElement element,
		                XmlWriter writer, ImmutableArray<object> services, IDisposable finish)
			: this(selector, namespaces, container, element, container, writer, services, finish) {}

		XmlWriteContext(IElements selector, INamespaces namespaces,
		                IContainerElement container, IElement element, IElement selected,
		                XmlWriter writer, ImmutableArray<object> services, IDisposable finish)
		{
			_selector = selector;
			_namespaces = namespaces;
			Container = container;
			Element = element;
			Selected = selected;
			_writer = writer;
			_services = services;
			_finish = finish;
		}

		public IContainerElement Container { get; }
		public IElement Element { get; }
		public IElement Selected { get; }

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

		public IDisposable Emit()
		{
			var display = Container as IName ?? Element as IName;
			if (display != null)
			{
				if (Container is IMemberElement)
				{
					_writer.WriteStartElement(display.DisplayName);
				}
				else
				{
					var ns = _namespaces.Get(display.Classification);
					_writer.WriteStartElement(display.DisplayName, ns.NamespaceName);
				}
				return _finish;
			}
			throw new SerializationException(
				$"Display name not found for element '{Element}' within a container of '{Container}.'");
		}

		public void Write(string text) => _writer.WriteString(text);

		public void Write(IName name, object value)
		{
			var type = value as TypeInfo;
			if (type != null) // TODO: Make this a selector.
			{
				_writer.WriteStartAttribute(name.DisplayName, _namespaces.Get(name.Classification).NamespaceName);
				_writer.WriteQualifiedName(TypeFormatter.Default.Format(type), _namespaces.Get(type).NamespaceName);
				_writer.WriteEndAttribute();
			}
		}

		public IWriteContext New(IContainerElement parameter, TypeInfo instanceType)
			=> Create(parameter, _selector.Load(parameter, instanceType));

		XmlWriteContext Create(IContainerElement container, IElement element) => Create(container, element, container);

		public IContext Select() => Create(Container, Element, Element);

		XmlWriteContext Create(IContainerElement container, IElement element, IElement selected)
			=> new XmlWriteContext(_selector, _namespaces, container, element, selected, _writer, _services, _finish);
	}
}