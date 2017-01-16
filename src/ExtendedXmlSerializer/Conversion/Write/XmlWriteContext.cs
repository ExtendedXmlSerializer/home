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
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Write
{
    class XmlWriteContainerContext : IWriteContainerContext
    {
        private readonly IWriteContext _context;
        public IDeclaredTypeElement Container { get; }
        public XmlWriteContainerContext(IDeclaredTypeElement container, IWriteContext context)
        {
            _context = context;
            Container = container;
        }

        public object GetService(Type serviceType) => _context.GetService(serviceType);

        public string DisplayName => _context.DisplayName;

        public TypeInfo Classification => _context.Classification;

        public void Dispose() => _context.Dispose();

        public IWriteContext Start(IElement element) => _context.Start(element);

        public void Write(string text) => _context.Write(text);

        public void Write(IElementName name, string value) => _context.Write(name, value);

        public IElement Element => _context.Element;
    }

    class XmlWriteContext : IWriteContext
    {
        private readonly IElementSelector _selector;
        private readonly INamespaces _namespaces;
        private readonly XmlWriter _writer;
        private readonly ImmutableArray<object> _services;
        private readonly IDisposable _finish, _disposable;

        public XmlWriteContext(XmlWriter writer) : this(writer, writer) {}

        public XmlWriteContext(XmlWriter writer, params object[] services)
            : this(Elements.Default, Namespaces.Default, writer, services) {}

        protected XmlWriteContext(IElementSelector selector, INamespaces namespaces, XmlWriter writer,
                                  params object[] services)
            : this(
                null, null, selector, namespaces, writer, null, services.ToImmutableArray(),
                new DelegatedDisposable(writer.WriteEndElement)) {}

        /*private XmlWriteContext(IElementSelector selector, INamespaces namespaces,
                                XmlWriter writer, IElement element, ImmutableArray<object> services, IDisposable finish,
                                IDisposable disposable = null)
            : this(
                element.Name.DisplayName,
                (element as IDeclaredTypeElement)?.DeclaredType ?? element.Name.Classification, selector, namespaces,
                writer, element, services, finish, disposable) {}*/

        XmlWriteContext(string displayName, TypeInfo effectiveType, IElementSelector selector,
                        INamespaces namespaces, XmlWriter writer,
                        IElement element, ImmutableArray<object> services, IDisposable finish,
                        IDisposable disposable = null)
        {
            DisplayName = displayName;
            Classification = effectiveType;
            Element = element;
            _selector = selector;
            _namespaces = namespaces;
            _writer = writer;
            _services = services;
            _finish = finish;
            _disposable = disposable;
        }

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
            var declared = element as IDeclaredTypeElement;
            var effective = declared?.DeclaredType ?? element.Name.Classification;
            var name = element.Name.DisplayName;
            var ns = _namespaces.Get(effective);
            _writer.WriteStartElement(name, ns);

            var subject = declared != null ? _selector.Get(declared.DeclaredType) : element;
            var context = new XmlWriteContext(name, effective, _selector, _namespaces, _writer, subject, _services,
                                              _finish, _finish);
            var parent = declared ?? Element as IDeclaredTypeElement;
            var result = parent != null ? (IWriteContext)new XmlWriteContainerContext(parent, context) : context;
            return result;
        }

        public void Write(string text) => _writer.WriteString(text);

        public void Write(IElementName element, string value)
            => _writer.WriteAttributeString(element.DisplayName, value);

        public virtual void Dispose() => _disposable?.Dispose();
    }
}