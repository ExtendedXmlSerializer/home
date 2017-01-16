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

using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class XmlReadContextFactory : IXmlReadContextFactory
    {
        public static XmlReadContextFactory Default { get; } = new XmlReadContextFactory();
        XmlReadContextFactory() : this(Elements.Default, ElementTypes.Default, NameConverter.Default) {}

        private readonly IElementSelector _selector;
        private readonly IElementTypes _types;
        private readonly INameConverter _converter;


        public XmlReadContextFactory(IElementSelector selector, IElementTypes types, INameConverter converter)
        {
            _selector = selector;
            _types = types;
            _converter = converter;
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

        public IReadContext Element(IContainerElement container, XElement data)
        {
            var name = _converter.Get(container);
            var native = data.Element(name);
            var result = native != null ? Create(container, native) : null;
            return result;
        }

        public IReadContext Create(IContainerElement container, XElement data)
        {
            var element = Create(data, container.DeclaredType);
            var member = container as IMemberElement;
            if (member != null)
            {
                return new XmlReadMemberContext(this, member, element, data);
            }

            var result = Create(container, element, data);
            return result;
        }

        private IElement Create(XElement data, TypeInfo declared) => _selector.Get(Initialized(data, declared));

        private IReadContext Create(IContainerElement container, IElement element, XElement data) =>
            new XmlReadContext(this, container, element, data, element.DisplayName);

        public string Value(IElementName name, XElement data)
        {
            var xName = _converter.Get(name);
            var value = data.Attribute(xName)?.Value;
            return value;
        }
    }
}