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
    class XmlWriteContext : IWriteContext
    {
        private readonly INamespaces _namespaces;
        private readonly XmlWriter _writer;
        private readonly ImmutableArray<object> _services;
        private readonly IDisposable _finish;

        public XmlWriteContext(INamespaces namespaces, XmlWriter writer, params object[] services)
        {
            _namespaces = namespaces;
            _writer = writer;
            _services = services.ToImmutableArray();
            _finish = new DelegatedDisposable(_writer.WriteEndElement);
        }

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

        public IWriteElementContext Start(IElement element)
        {
            var ns = _namespaces.Get(element.ReferencedType);
            _writer.WriteStartElement(element.Name, ns);

            var result = new WriteElementContext(this, element, _finish);
            return result;
        }

        public void Write(string text) => _writer.WriteString(text);

        public void Write(IElement element, string value) => _writer.WriteAttributeString(element.Name, value);

        class WriteElementContext : IWriteElementContext
        {
            private readonly IWriteContext _context;
            private readonly IDisposable _disposable;

            public WriteElementContext(IWriteContext context, IElement element, IDisposable disposable)
            {
                _context = context;
                _disposable = disposable;
                Current = element;
            }

            public object GetService(Type serviceType) => _context.GetService(serviceType);

            public IWriteElementContext Start(IElement element) => _context.Start(element);

            public void Write(string text) => _context.Write(text);

            public void Write(IElement element, string value) => _context.Write(element, value);

            public void Dispose() => _disposable.Dispose();
            public IElement Current { get; }
        }
    }
}