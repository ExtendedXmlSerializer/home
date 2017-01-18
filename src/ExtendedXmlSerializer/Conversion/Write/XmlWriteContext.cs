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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Write
{
    class XmlWriteContext : IWriteContext
    {
        private readonly IElements _selector;
        private readonly INamespaces _namespaces;
        private readonly XmlWriter _writer;
        private readonly ImmutableArray<object> _services;
        private readonly IDisposable _finish;

        public XmlWriteContext(XmlWriter writer, IRoot root) : this(writer, root, writer) {}

        public XmlWriteContext(XmlWriter writer, IRoot root, params object[] services)
            : this(Elements.Default, Namespaces.Default, writer, root, services) {}

        public XmlWriteContext(IElements selector, INamespaces namespaces, XmlWriter writer, IRoot root,
                               params object[] services)
            : this(
                selector, namespaces, root, selector.Get(root.Classification), writer, services.ToImmutableArray(),
                new DelegatedDisposable(writer.WriteEndElement)) {}

        private XmlWriteContext(IElements selector, INamespaces namespaces,
                                IContainerElement container, IElement element,
                                XmlWriter writer, ImmutableArray<object> services, IDisposable finish)
            : this(null, selector, namespaces, container, element, container, writer, services, finish) {}

        XmlWriteContext(IContext parent, IElements selector, INamespaces namespaces,
                        IContainerElement container, IElement element, IElement selected,
                        XmlWriter writer, ImmutableArray<object> services, IDisposable finish)
        {
            _selector = selector;
            _namespaces = namespaces;
            Parent = parent;
            Container = container;
            Element = element;
            Selected = selected;
            _writer = writer;
            _services = services;
            _finish = finish;
        }

        public IContext Parent { get; }
        public IContainerElement Container { get; }
        public IElement Element { get; }
        public IElement Selected { get; }

        public object GetService(Type serviceType)
        {
            var info = serviceType.GetTypeInfo();
            var length = _services.Length;
            for (int i = 0; i < length; i++)
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
            var ns = _namespaces.Get(Element.Classification);
            var name = (Container as IDisplayAware)?.DisplayName ?? (Element as IDisplayAware)?.DisplayName;
            if (name != null)
            {
                _writer.WriteStartElement(name, ns);
                return _finish;
            }
            throw new SerializationException(
                $"Display name not found for element '{Element}' in a container of '{Container}.'");
        }

        public void Write(string text) => _writer.WriteString(text);

        public void Write(IElementName element, string value)
            => _writer.WriteAttributeString(element.DisplayName, value);

        public IWriteContext New(IContainerElement parameter, TypeInfo instanceType)
            => Create(parameter, _selector.Get(instanceType));

        private XmlWriteContext Create(IContainerElement container, IElement element)
            => Create(container, element, container);

        public IContext Select() => Create(Container, Element, Element);

        private XmlWriteContext Create(IContainerElement container, IElement element, IElement selected)
            => new XmlWriteContext(this, _selector, _namespaces, container, element, selected, _writer, _services, _finish);
    }
}