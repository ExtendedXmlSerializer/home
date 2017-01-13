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
        private readonly IMembers _store;
        private readonly IMemberElements _members;
        private readonly IElementTypeLocator _locator;
        private readonly IElementTypes _types;
        private readonly INameConverter _names;
        private readonly XElement _element;

        public XmlReadContext(XElement element)
            : this(
                Members.Members.Default, ElementTypeLocator.Default, ElementTypes.Default, NameConverter.Default,
                element,
                Members.Members.Default, ElementTypeLocator.Default, ElementTypes.Default, Namespaces.Default) {}

        public XmlReadContext(IMembers store, IElementTypeLocator locator, IElementTypes types,
                              INameConverter namespaces, XElement element, params object[] services)
            : this(store, locator, types, namespaces, element)
        {
            for (int i = 0; i < services.Length; i++)
            {
                Add(services[i]);
            }
        }

        private XmlReadContext(IMembers store, IElementTypeLocator locator, IElementTypes types,
                               INameConverter namespaces, XElement element)
            : this(store, locator, types, namespaces, new ElementName(types.Get(element), element.Name.LocalName), element) {}

        private XmlReadContext(IMembers store, IElementTypeLocator locator, IElementTypes types,
                               INameConverter namespaces, IElementName name, XElement element)
            : this(store, store.Get(name.ReferencedType), locator, types, namespaces, name, element) {}

        XmlReadContext(IMembers store, IMemberElements members, IElementTypeLocator locator,
                       IElementTypes types, INameConverter names, IElementName name, XElement element)
        {
            _store = store;
            _members = members;
            _locator = locator;
            _types = types;
            _names = names;
            Name = name;
            _element = element;
            Add(element);
        }

        public IElementName Name { get; }

        public object GetService(Type serviceType) => _element.AnnotationAll(serviceType);

        public void Add(object service) => _element.AddAnnotation(service);

        public string Read() => _element.Value;

        public IReadContext Member(IElement element)
        {
            var name = _names.Get(element.Name);
            var native = _element.Element(name);
            var result = Create(native, (element as IDeclaredTypeElement)?.DeclaredType);
            return result;
        }

        public IEnumerable<IReadContext> Items()
        {
            var elementType = _locator.Get(Name.ReferencedType);
            foreach (var child in _element.Elements())
            {
                yield return Create(child, elementType);
            }
        }

        private XmlReadContext Create(XElement child, TypeInfo elementType = null) =>
            new XmlReadContext(_store, _locator, _types, _names, _types.Initialized(child, elementType));

        public IEnumerable<IReadContext> ChildrenOf(IElementName name)
        {
            foreach (var child in _element.Elements(_names.Get(name)))
            {
                yield return Create(child, Name.ReferencedType);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IReadContext> GetEnumerator()
        {
            foreach (var child in _element.Elements())
            {
                var memberType = _members.Get(child.Name.LocalName)?.DeclaredType;
                if (memberType != null)
                {
                    yield return Create(child, memberType);
                }
            }
        }

        public string this[IElementName name]
        {
            get
            {
                var n = _names.Get(name);
                var result = _element.Attribute(n)?.Value;
                return result;
            }
        }
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

        public XName Get(IElementName parameter) => XName.Get(parameter.Name, _namespaces.Get(parameter.ReferencedType));
    }
}