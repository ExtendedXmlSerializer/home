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
        private readonly IElementSelector _selector;
        private readonly INamespaces _namespaces;
        private readonly XmlWriter _writer;
        private readonly ImmutableArray<object> _services;
        private readonly IDisposable _finish;

        public XmlWriteContext(XmlWriter writer) : this(writer, writer) {}

        public XmlWriteContext(XmlWriter writer, params object[] services)
            : this(Elements.Default, Namespaces.Default, writer, services) {}

        protected XmlWriteContext(IElementSelector selector, INamespaces namespaces, XmlWriter writer,
                                  params object[] services)
            : this(
                null, null, null, selector, namespaces, writer, null, services.ToImmutableArray(),
                new DelegatedDisposable(writer.WriteEndElement)) {}

        /*private XmlWriteContext(IElementSelector selector, INamespaces namespaces,
                                XmlWriter writer, IElement element, ImmutableArray<object> services, IDisposable finish,
                                IDisposable disposable = null)
            : this(
                element.Name.DisplayName,
                (element as IDeclaredTypeElement)?.DeclaredType ?? element.Name.Classification, selector, namespaces,
                writer, element, services, finish, disposable) {}*/

        XmlWriteContext(IContainerElement container, string displayName, TypeInfo effectiveType,
                        IElementSelector selector,
                        INamespaces namespaces, XmlWriter writer,
                        IElement element, ImmutableArray<object> services, IDisposable finish)
        {
            Container = container;
            DisplayName = displayName;
            Classification = effectiveType;
            Element = element;
            _selector = selector;
            _namespaces = namespaces;
            _writer = writer;
            _services = services;
            _finish = finish;
        }

        public IContainerElement Container { get; }
        public string DisplayName { get; }
        public TypeInfo Classification { get; }
        public IElement Element { get; }

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

        public IWriteContext Start(IElement element)
        {
            var declared = element as IContainerElement;
            var effective = declared?.DeclaredType ?? element.Name.Classification;
            var name = element.Name.DisplayName;
            var ns = _namespaces.Get(effective);
            _writer.WriteStartElement(name, ns);

            var subject = declared != null ? _selector.Get(declared.DeclaredType) : element;
            var container = declared ?? Element as IContainerElement;
            var result = new XmlWriteContext(container, name, effective, _selector, _namespaces, _writer, subject,
                                             _services, _finish);
            return result;
        }

        public void Write(string text) => _writer.WriteString(text);

        public void Write(IElementName element, string value)
            => _writer.WriteAttributeString(element.DisplayName, value);

        public virtual void Dispose() => _finish.Dispose();
    }
}