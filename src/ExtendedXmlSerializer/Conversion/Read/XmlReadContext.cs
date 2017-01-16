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

        public XmlReadContext(IElementSelector selector, IElementTypes types, INameConverter namespaces, XElement data,
                              IElement element, params object[] services)
            : base(selector, types, namespaces, data, element, services) {}

        public XmlReadContext(IContainerElement container, IElementSelector selector, IElementTypes types,
                              INameConverter names, XElement data, IElement element, string displayName)
            : base(container, selector, types, names, data, element, displayName) {}
    }

    public abstract class XmlReadContext<T> : IReadContainerContext<T> where T : class, IContainerElement
    {
        private readonly IElementSelector _selector;
        private readonly IElementTypes _types;
        private readonly INameConverter _names;
        private readonly XElement _data;

        public XmlReadContext(XElement data)
            : this(
                Elements.Default, ElementTypes.Default, NameConverter.Default, data,
                Elements.Default.Get(ElementTypes.Default.Get(data))) {}

        public XmlReadContext(IElementSelector selector, IElementTypes types, INameConverter namespaces, XElement data,
                              IElement element, params object[] services)
            : this(null, selector, types, namespaces, data, element, element.Name.DisplayName)
        {
            for (int i = 0; i < services.Length; i++)
            {
                Add(services[i]);
            }
        }

        protected XmlReadContext(T container, IElementSelector selector, IElementTypes types, INameConverter names,
                                 XElement data, IElement element, string displayName)
            : this(container, selector, types, names, data, element, displayName, element.Name.Classification) {}

        XmlReadContext(T container, IElementSelector selector, IElementTypes types, INameConverter names, XElement data,
                       IElement element, string displayName, TypeInfo classification)
        {
            _selector = selector;
            _types = types;
            _names = names;
            Container = container;
            Element = element;
            DisplayName = displayName;
            Classification = classification;
            _data = data;
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

        public IReadContext Member(IContainerElement element)
        {
            var name = _names.Get(element.Name);
            var native = _data.Element(name);
            var result = native != null ? Create(element, native) : null;
            return result;
        }

        public IEnumerable<IReadContext> Items()
        {
            var container = ((ICollectionElement) Element).Item;
            foreach (var child in _data.Elements())
            {
                yield return Create(container, child);
            }
        }

        private TypeInfo Initialized(XElement source, TypeInfo declared)
        {
            var info = _types.Get(source);
            if (info == null)
            {
                source.Annotated(declared);
                return declared;
            }
            return info;
        }

        private IReadContext Create(IContainerElement container, XElement data)
            => Create(container, Create(data, container.DeclaredType), data);

        private IElement Create(XElement data, TypeInfo declared) => _selector.Get(Initialized(data, declared));

        private IReadContext Create(IContainerElement container, IElement element, XElement data) =>
            new XmlReadContext(container, _selector, _types, _names, data, element, element.Name.DisplayName);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IReadMemberContext> GetEnumerator()
        {
            var members = ((IMemberedElement) Element).Members;
            foreach (var source in _data.Elements())
            {
                var member = members.Get(source.Name.LocalName);
                if (member != null)
                {
                    var element = Create(source, member.DeclaredType);
                    yield return
                        new XmlReadMemberContext(member, _selector, _types, _names, source, element,
                                                 member.Name.DisplayName);
                }
            }
        }

        public string this[IElementName name]
        {
            get
            {
                var xName = _names.Get(name);
                var value = _data.Attribute(xName)?.Value;
                return value;
            }
        }
    }
}