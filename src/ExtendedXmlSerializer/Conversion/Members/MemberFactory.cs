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
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Legacy;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;

namespace ExtendedXmlSerialization.Conversion.Members
{
    public class MemberFactory : IMemberFactory
    {
        private readonly IEnumeratingReader _reader;
        private readonly IElementTypes _elementTypes;
        private readonly IConverter _converter;
        private readonly IElementProvider _element;
        private readonly IGetterFactory _getter;
        private readonly ISetterFactory _setter;
        private readonly IAddDelegates _add;

        public MemberFactory(IElementTypes elementTypes, IConverter converter, IEnumeratingReader reader)
            : this(elementTypes, converter, reader, GetterFactory.Default) {}

        public MemberFactory(IElementTypes elementTypes, IConverter converter, IEnumeratingReader reader,
                             IGetterFactory getter)
            : this(elementTypes, converter, reader, MemberElementProvider.Default, getter,
                   SetterFactory.Default, AddDelegates.Default) {}

        public MemberFactory(IElementTypes elementTypes, IConverter converter, IEnumeratingReader reader,
                             IElementProvider element,
                             IGetterFactory getter, ISetterFactory setter, IAddDelegates add)
        {
            _elementTypes = elementTypes;
            _converter = converter;
            _reader = reader;
            _element = element;
            _getter = getter;
            _setter = setter;
            _add = add;
        }

        public IMember Create(MemberInfo metadata, Typing memberType, bool assignable)
        {
            var element = _element.Get(metadata);
            var getter = _getter.Get(metadata);

            if (assignable)
            {
                var type = new TypeEmittingWriter(new EmitTypeSpecification(memberType), _converter);
                var writer = new InstanceValidatingWriter(new ElementWriter(element, type));
                var result = new AssignableMember(new InitializingReader(_elementTypes, _converter, memberType),
                                                  writer, element, getter, _setter.Get(metadata));
                return result;
            }

            var add = _add.Get(memberType);
            if (add != null)
            {
                var writer = new ElementWriter(element, _converter);
                var result = new ReadOnlyCollectionMember(new InitializingReader(_elementTypes, _reader, memberType),
                                                          writer, element, getter, add);
                return result;
            }
            return null;
        }
    }
}