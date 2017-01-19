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

        private readonly IElements _selector;
        private readonly IElementTypes _types;
        private readonly INameConverter _converter;

        public XmlReadContextFactory(IElements selector, IElementTypes types, INameConverter converter)
        {
            _selector = selector;
            _types = types;
            _converter = converter;
        }

        private TypeInfo Initialized(IContext context, IClassification classification, XElement data)
        {
            var info = _types.Get(data);
            if (info == null)
            {
                var result = context.Container.GetDeclaredType(classification);
                data.Annotated(result);
                return result;
            }
            return info;
        }

        public IReadContext Create(IReadContext context, IContainerElement container, XElement data)
        {
            var typeInfo = Initialized(context, container, data);
            var element = _selector.Load(container, typeInfo);
            var member = container as IMemberElement;
            if (member != null)
            {
                return new XmlReadMemberContext(this, member, element, data);
            }

            var result = new XmlReadContext(this, container, element, data);
            return result;
        }

        public string Value(IElementName name, XElement data)
        {
            var xName = _converter.Get(name);
            var value = data.Attribute(xName)?.Value;
            return value;
        }

        public IContext Select(IReadContext context, XElement data) =>
            new XmlReadContext(this, context.Container, context.Element, context.Element, data);
    }
}