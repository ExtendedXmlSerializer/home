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
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class XmlReadContext : IReadContext
    {
        private readonly IElementSelector _selector;
        private readonly IElementTypes _types;
        private readonly INameConverter _names;
        private readonly XElement _container;
        private readonly IElementName _name;

        public XmlReadContext(XElement container) : this(Elements.Default, container) {}

        XmlReadContext(IElementSelector selector, XElement container)
            : this(
                selector, ElementTypes.Default, NameConverter.Default, container,
                selector.Get(ElementTypes.Default.Get(container)), ElementTypes.Default) {}

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
                               IElement element) : this(selector, types, names, container, element, element.Name) {}

        XmlReadContext(IElementSelector selector, IElementTypes types, INameConverter names, XElement container,
                       IElement element, IElementName name)
        {
            _selector = selector;
            _types = types;
            _names = names;
            Current = element;
            _container = container;
            _name = name;
            Add(container);
        }

        public string DisplayName => _name.DisplayName;

        public TypeInfo Classification => _name.Classification;


        /*public IReadContext Parent { get; }*/
        public IElement Current { get; }

        public object GetService(Type serviceType) => _container.AnnotationAll(serviceType);

        public void Add(object service) => _container.AddAnnotation(service);

        public string Read() => _container.Value;

        public IReadContext Member(IElement element)
        {
            var name = _names.Get(element.Name);
            var native = _container.Element(name);
            var result = Create(native, element.EffectiveType());
            return result;
        }

        public IEnumerable<IReadContext> Items()
        {
            var collection = (ICollectionElement) _selector.Get(Current.EffectiveType());
            var elementType = collection.Item.DeclaredType;
            foreach (var child in _container.Elements())
            {
                yield return Create(child, elementType);
            }
        }

        /*private XmlReadContext Create(XElement source, TypeInfo elementType = null)
        {
            var container = Initialized(source, elementType ?? Current.EffectiveType());
            var typeInfo = _types.Get(container);
            var element = _selector.Get(typeInfo);
            return Create(element, container);
        }*/

        /*private XmlReadContext Create(IElement element, XElement source)
        {
            var child = CreateChild(element, source);
            var declared = element as IDeclaredTypeElement;
            var result = declared != null
                ? child.CreateChild(_selector.Get(declared.DeclaredType), source)
                : child;
            return result;
        }*/

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

        private XmlReadContext Create(XElement source, TypeInfo declared, IElementName name = null)
        {
            var info = Initialized(source, declared);
            var element = _selector.Get(info);
            return Create(source, element, name ?? element.Name);
        }

        private XmlReadContext Create(XElement source, IElement element, IElementName name)
        {
            var child = new XmlReadContext(_selector, _types, _names, source, element, name);
            return child;
        }

        /*public IEnumerable<IReadContext> ChildrenOf(IElementName name)
        {
            foreach (var child in _container.Elements(_names.Get(name)))
            {
                yield return Create(child, Current.Name.Classification);
            }
        }*/

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IReadContext> GetEnumerator()
        {
            var members = ((IMemberedElement) Current).Members;
            foreach (var source in _container.Elements())
            {
                var member = members.Get(source.Name.LocalName);
                if (member != null)
                {
                    yield return Create(source, member.DeclaredType, member.Name);
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