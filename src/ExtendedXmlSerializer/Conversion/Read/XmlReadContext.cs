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

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class XmlReadContext : IReadContext
    {
        private readonly IElementSelector _selector;
        private readonly IElementTypes _types;
        private readonly INameConverter _names;
        private readonly XElement _container;

        public XmlReadContext(XElement container)
            : this(
                Elements.Default, ElementTypes.Default, NameConverter.Default, container,
                Elements.Default.Get(ElementTypes.Default.Get(container)), ElementTypes.Default) {}

        public XmlReadContext(IElementSelector selector, IElementTypes types, INameConverter namespaces,
                              XElement container, IElement element, params object[] services)
            : this(selector, types, namespaces, container, element)
        {
            for (int i = 0; i < services.Length; i++)
            {
                Add(services[i]);
            }
        }

        private XmlReadContext(IElementSelector selector, IElementTypes types, INameConverter names, XElement container,
                               IElement element)
            : this(selector, types, names, container, element, element.Name.DisplayName, element.Name.Classification) {}

        XmlReadContext(IElementSelector selector, IElementTypes types, INameConverter names, XElement container,
                       IElement element, string displayName, TypeInfo classification)
        {
            _selector = selector;
            _types = types;
            _names = names;
            Element = element;
            DisplayName = displayName;
            Classification = classification;
            _container = container;
            Add(container);
        }

        public string DisplayName { get; }

        public TypeInfo Classification { get; }

        public IElement Element { get; }

        public object GetService(Type serviceType) => _container.AnnotationAll(serviceType);

        public void Add(object service) => _container.AddAnnotation(service);

        public string Read() => _container.Value;

        public IReadContext Member(IElement element)
        {
            var name = _names.Get(element.Name);
            var native = _container.Element(name);
            if (native != null)
            {
                var type = (element as IDeclaredTypeElement)?.DeclaredType ?? element.Name.Classification;
                var result = Create(native, type);
                return result;
            }
            return null;
        }

        public IEnumerable<IReadContext> Items()
        {
            var elementType = ((ICollectionElement) Element).Item.DeclaredType;
            foreach (var child in _container.Elements())
            {
                yield return Create(child, elementType);
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

        private XmlReadContext Create(XElement source, TypeInfo declared, string displayName = null)
        {
            var info = Initialized(source, declared);
            var element = _selector.Get(info);
            var result = new XmlReadContext(_selector, _types, _names, source, element,
                                            displayName ?? element.Name.DisplayName, element.Name.Classification);
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IReadMemberContext> GetEnumerator()
        {
            var members = ((IMemberedElement) Element).Members;
            foreach (var source in _container.Elements())
            {
                var member = members.Get(source.Name.LocalName);
                if (member != null)
                {
                    var context = Create(source, member.DeclaredType, member.Name.DisplayName);
                    yield return new XmlReadMemberContext(member, context);
                }
            }
        }

        public string this[IElementName name]
        {
            get
            {
                var xName = _names.Get(name);
                var value = _container.Attribute(xName)?.Value;
                return value;
            }
        }
    }
}