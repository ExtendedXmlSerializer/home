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
using System.Collections.Generic;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class XmlReadContext : IReadContext
    {
        private readonly IElementTypeLocator _locator;
        private readonly IElementTypes _types;
        private readonly INamespaces _namespaces;
        private readonly XElement _element;

        public XmlReadContext(XElement element)
            : this(ElementTypeLocator.Default, ElementTypes.Default, Namespaces.Default, element) {}

        public XmlReadContext(IElementTypeLocator locator, IElementTypes types, INamespaces namespaces, XElement element)
            : this(locator, types, namespaces, types.Get(element), element.Name.LocalName, element, element) {}

        XmlReadContext(IElementTypeLocator locator, IElementTypes types, INamespaces namespaces, Typing ownerType,
                       string name, XElement element, params object[] services)
        {
            _locator = locator;
            _types = types;
            _namespaces = namespaces;
            OwnerType = ownerType;
            Name = name;
            _element = element;

            for (int i = 0; i < services.Length; i++)
            {
                Add(services[i]);
            }
        }

        public Typing OwnerType { get; }
        public string Name { get; }

        public object GetService(Type serviceType) => _element.Annotation(serviceType);

        public void Add(object service) => _element.AddAnnotation(service);

        public string Read() => _element.Value;

        public IReadContext Member(IElement element, Type hint = null)
        {
            var name = ElementName(element);
            var result = new XmlReadContext(_locator, _types, _namespaces,
                                            _types.Initialized(_element.Element(name), hint));
            return result;
        }

        public IEnumerable<IReadContext> Members(IMembers members)
        {
            foreach (var child in _element.Elements())
            {
                var member = members.Get(child.Name.LocalName);
                if (member != null)
                {
                    yield return
                        new XmlReadContext(_locator, _types, _namespaces,
                                           _types.Initialized(child, member.MemberType));
                }
            }
        }

        public IEnumerable<IReadContext> Items()
        {
            var elementType = _locator.Get(OwnerType);
            foreach (var child in _element.Elements())
            {
                yield return new XmlReadContext(_locator, _types, _namespaces, _types.Initialized(child, elementType));
            }
        }

        public IEnumerable<IReadContext> Children(IElement element)
        {
            var name = ElementName(element);
            foreach (var child in _element.Elements(name))
            {
                yield return new XmlReadContext(_locator, _types, _namespaces, child);
            }
        }

        private XName ElementName(IElement element) => XName.Get(element.Name, _namespaces.Get(element.OwnerType));
    }
}