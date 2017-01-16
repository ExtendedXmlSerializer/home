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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class XmlReadContext : XmlReadContext<IContainerElement>
    {
        public XmlReadContext(XElement data) : base(data) {}

        public XmlReadContext(IXmlReadContextFactory factory, IElement element, XElement data, params object[] services)
            : base(factory, element, data, services) {}

        public XmlReadContext(IXmlReadContextFactory factory, IContainerElement container, IElement element,
                              XElement data, string displayName)
            : base(factory, container, element, data, displayName) {}
    }

    public abstract class XmlReadContext<T> : IReadContainerContext<T> where T : class, IContainerElement
    {
        private readonly IXmlReadContextFactory _factory;
        private readonly XElement _data;

        public XmlReadContext(XElement data)
            : this(XmlReadContextFactory.Default, Elements.Default.Get(ElementTypes.Default.Get(data)), data) {}

        public XmlReadContext(IXmlReadContextFactory factory, IElement element, XElement data, params object[] services)
            : this(factory, null, element, data, element.DisplayName)
        {
            for (int i = 0; i < services.Length; i++)
            {
                Add(services[i]);
            }
        }

        protected XmlReadContext(IXmlReadContextFactory factory, T container, IElement element, XElement data)
            : this(factory, container, element, data, element.DisplayName) {}

        protected XmlReadContext(IXmlReadContextFactory factory, T container, IElement element, XElement data,
                                 string displayName)
            : this(factory, container, element, data, displayName, element.Classification) {}

        XmlReadContext(IXmlReadContextFactory factory, T container, IElement element, XElement data,
                       string displayName, TypeInfo classification)
        {
            Container = container;
            _factory = factory;
            _data = data;
            Element = element;
            DisplayName = displayName;
            Classification = classification;
            Add(data);
        }

        public string DisplayName { get; }

        public TypeInfo Classification { get; }

        public T Container { get; }
        IContainerElement IReadContainerContext.Container => Container;

        public IElement Element { get; }

        public object GetService(Type serviceType) => _data.AnnotationAll(serviceType);

        public void Add(object service) => _data.AddAnnotation(service);

        public string Read() => _data.Value;

        public IReadContext Member(IContainerElement element) => _factory.Element(element, _data);

        public IEnumerable<IReadContext> Items()
        {
            var container = ((ICollectionElement) Element).Item;
            foreach (var child in _data.Elements())
            {
                yield return _factory.Create(container, child);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IReadMemberContext> GetEnumerator()
        {
            var members = ((IMemberedElement) Element).Members;
            foreach (var source in _data.Elements())
            {
                var member = members.Get(source.Name.LocalName);
                if (member != null)
                {
                    yield return (IReadMemberContext) _factory.Create(member, source);
                }
            }
        }

        public string this[IElementName name] => _factory.Value(name, _data);
    }
}