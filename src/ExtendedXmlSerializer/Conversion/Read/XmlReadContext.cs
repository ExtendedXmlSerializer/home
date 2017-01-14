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
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class XmlReadContext : IReadContext
    {
        private readonly IElementSelector _elements;
        private readonly IElementMembers _store;
        private readonly IMembers _members;
        private readonly ICollectionItemTypeLocator _locator;
        private readonly IElementTypes _types;
        private readonly INameConverter _names;
        private readonly XElement _container;

        public XmlReadContext(XElement container) : this(Elements.Default, container) {}

        XmlReadContext(IElementSelector elements, XElement container)
            : this(
                elements, ElementMembers.Default, CollectionItemTypeLocator.Default, ElementTypes.Default,
                NameConverter.Default,
                container, elements.Get(ElementTypes.Default.Get(container)),
                ElementMembers.Default, CollectionItemTypeLocator.Default, ElementTypes.Default,
                Namespaces.Default) {}

        public XmlReadContext(IElementSelector elements, IElementMembers store, ICollectionItemTypeLocator locator,
                              IElementTypes types,
                              INameConverter namespaces, XElement container, IElement element, params object[] services)
            : this(elements, store, locator, types, namespaces, container, element)
        {
            for (int i = 0; i < services.Length; i++)
            {
                Add(services[i]);
            }
        }

        private XmlReadContext(IElementSelector elements, IElementMembers store, ICollectionItemTypeLocator locator,
                               IElementTypes types,
                               INameConverter namespaces, XElement container, IElement element)
            : this(
                elements, store, store.Get(element.Name.Classification), locator, types, namespaces, container, element) {}

        XmlReadContext(IElementSelector elements, IElementMembers store, IMembers members,
                       ICollectionItemTypeLocator locator,
                       IElementTypes types, INameConverter names, XElement container, IElement element)
        {
            _elements = elements;
            _store = store;
            _members = members;
            _locator = locator;
            _types = types;
            _names = names;
            Current = element;
            _container = container;
            Add(element);
        }

        public IElement Current { get; }

        public object GetService(Type serviceType) => _container.AnnotationAll(serviceType);

        public void Add(object service) => _container.AddAnnotation(service);

        public string Read() => _container.Value;

        public IReadContext Member(IElement element)
        {
            var name = _names.Get(element.Name);
            var native = _container.Element(name);
            var result = Create(native, (element as IDeclaredTypeElement)?.DeclaredType);
            return result;
        }

        public IEnumerable<IReadContext> Items()
        {
            var elementType = ((ICollectionElement) Current).Item.ElementType;
            foreach (var child in _container.Elements())
            {
                yield return Create(child, elementType);
            }
        }

        private XmlReadContext Create(XElement child, TypeInfo elementType = null) =>
            new XmlReadContext(_elements, _store, _locator, _types, _names, _types.Initialized(child, elementType),
                               _elements.Get(_types.Get(child)));

        public IEnumerable<IReadContext> ChildrenOf(IElementName name)
        {
            foreach (var child in _container.Elements(_names.Get(name)))
            {
                yield return Create(child, Current.Name.Classification);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IReadContext> GetEnumerator()
        {
            foreach (var child in _container.Elements())
            {
                var memberType = _members.Get(child.Name.LocalName)?.DeclaredType;
                if (memberType != null)
                {
                    yield return Create(child, memberType);
                }
            }
        }

        public string this[IElementName name] => _container.Attribute(_names.Get(name))?.Value;
    }

    public interface INameConverter : IParameterizedSource<IElementName, XName> {}

    class NameConverter : INameConverter
    {
        public static NameConverter Default { get; } = new NameConverter();
        NameConverter() : this(Namespaces.Default) {}

        private readonly INamespaces _namespaces;

        public NameConverter(INamespaces namespaces)
        {
            _namespaces = namespaces;
        }

        public XName Get(IElementName parameter)
            => XName.Get(parameter.DisplayName, _namespaces.Get(parameter.Classification));
    }
}