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
        // private readonly IElementSelector _selector;
        private readonly IElementSelector _selector;
        private readonly INamespaces _namespaces;
        private readonly XmlWriter _writer;
        private readonly ImmutableArray<object> _services;
        private readonly IDisposable _finish;

        public XmlWriteContext(XmlWriter writer, IRoot root) : this(writer, root, writer) {}

        public XmlWriteContext(XmlWriter writer, IRoot root, params object[] services)
            : this(Elements.Default, Namespaces.Default, writer, root, services) {}

        public XmlWriteContext(IElementSelector selector, INamespaces namespaces, XmlWriter writer, IRoot root,
                               params object[] services)
            : this(
                selector, namespaces, root, selector.Get(root.DeclaredType), writer, services.ToImmutableArray(),
                new DelegatedDisposable(writer.WriteEndElement)) {}

        /*private XmlWriteContext(INamespaces namespaces, IRoot root, XmlWriter writer, ImmutableArray<object> services,
                                IDisposable finish)
            : this(namespaces, root, writer, services, finish) {}*/

        XmlWriteContext(IElementSelector selector, INamespaces namespaces, IContainerElement container, IElement element,
                        XmlWriter writer,
                        ImmutableArray<object> services, IDisposable finish)
        {
            _selector = selector;
            _namespaces = namespaces;
            Container = container;
            Element = element;
            _writer = writer;
            _services = services;
            _finish = finish;
        }

        public IContainerElement Container { get; }
        public IElement Element { get; }

        public string DisplayName => Container.DisplayName;
        public TypeInfo Classification => Element.Classification;

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

        public IWriteContext Emit(TypeInfo instanceType)
        {
            var ns = _namespaces.Get(Classification);
            var element = _selector.Get(instanceType);
            var name = Container is ICollectionItem && !Equals(Container.DeclaredType, instanceType) ? element.DisplayName : DisplayName;
            _writer.WriteStartElement(name, ns);

            var result = new XmlWriteContext(_selector, _namespaces, Container, element, _writer, _services, _finish);
            return result;
        }

        public void Write(string text) => _writer.WriteString(text);

        public void Write(IElementName element, string value)
            => _writer.WriteAttributeString(element.DisplayName, value);

        public virtual void Dispose() => _finish.Dispose();

        public IWriteContext New(IContainerElement parameter)
            => new XmlWriteContext(_selector, _namespaces, parameter, parameter, _writer, _services, _finish);
    }
}