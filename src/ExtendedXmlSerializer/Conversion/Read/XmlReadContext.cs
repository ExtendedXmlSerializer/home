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
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class XmlReadContext : IReadContext
    {
        private readonly IMemberInformationProvider _information;
        private readonly IMemberElements _view;
        private readonly IElementTypeLocator _locator;
        private readonly IElementTypes _types;
        private readonly INamespaces _namespaces;
        private readonly XElement _element;

        public XmlReadContext(XElement element)
            : this(
                MemberInformationProvider.Default, ElementTypeLocator.Default, ElementTypes.Default, Namespaces.Default,
                element,
                MemberInformationProvider.Default, ElementTypeLocator.Default, ElementTypes.Default, Namespaces.Default) {}

        public XmlReadContext(IMemberInformationProvider information, IElementTypeLocator locator, IElementTypes types,
                              INamespaces namespaces, XElement element, params object[] services)
            : this(information, locator, types, namespaces, element)
        {
            for (int i = 0; i < services.Length; i++)
            {
                Add(services[i]);
            }
        }

        private XmlReadContext(IMemberInformationProvider information, IElementTypeLocator locator, IElementTypes types,
                               INamespaces namespaces, XElement element)
            : this(information, locator, types, namespaces, types.Get(element), element) {}

        private XmlReadContext(IMemberInformationProvider information, IElementTypeLocator locator, IElementTypes types,
                               INamespaces namespaces, Typing ownerType, XElement element)
            : this(information, information.Get(ownerType), locator, types, namespaces, ownerType, element) {}

        XmlReadContext(IMemberInformationProvider information, IMemberElements view, IElementTypeLocator locator,
                       IElementTypes types, INamespaces namespaces, Typing ownerType, XElement element)
        {
            _information = information;
            _view = view;
            _locator = locator;
            _types = types;
            _namespaces = namespaces;
            ReferencedType = ownerType;
            Name = element.Name.LocalName;
            _element = element;
            Add(element);
        }

        public Typing ReferencedType { get; }
        public string Name { get; }

        public object GetService(Type serviceType) => _element.AnnotationAll(serviceType);

        public void Add(object service) => _element.AddAnnotation(service);

        public string Read() => _element.Value;

        public IReadContext Member(IElement element, Type hint = null)
        {
            var name = ElementName(element);
            var native = _element.Element(name);
            var result = Create(native, hint);
            return result;
        }

        public IEnumerable<IReadContext> Items()
        {
            var elementType = _locator.Get(ReferencedType);
            foreach (var child in _element.Elements())
            {
                yield return Create(child, elementType);
            }
        }

        private XmlReadContext Create(XElement child, Type elementType = null) =>
            new XmlReadContext(_information, _locator, _types, _namespaces, _types.Initialized(child, elementType));

        public IEnumerable<IReadContext> ChildrenOf(IElement element)
        {
            var name = ElementName(element);
            foreach (var child in _element.Elements(name))
            {
                yield return Create(child, element.ReferencedType);
            }
        }

        private XName ElementName(IElement element) => XName.Get(element.Name, _namespaces.Get(element.ReferencedType));

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IReadContext> GetEnumerator()
        {
            foreach (var child in _element.Elements())
            {
                var memberType = _view.Get(child.Name.LocalName)?.DeclaredType;
                if (memberType != null)
                {
                    yield return Create(child, memberType);
                }
            }
        }

        public string this[IElement element]
        {
            get
            {
                var name = ElementName(element);
                var result = _element.Attribute(name)?.Value;
                return result;
            }
        }
    }
}