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
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    class LegacyInstanceTypeConverter : TypeConverter
    {
        public LegacyInstanceTypeConverter(ISerializationToolsFactory tools, IElementTypes elementTypes, IConverter converter)
            : this(tools, IsActivatedTypeSpecification.Default, elementTypes, converter) {}

        protected LegacyInstanceTypeConverter(ISerializationToolsFactory tools, ISpecification<TypeInfo> specification,
                                              IElementTypes elementTypes,
                                              IConverter converter)
            : this(
                specification,
                new InstanceMembers(new LegacyMemberFactory(tools,
                                                            new MemberFactory(elementTypes, converter,
                                                                              new EnumeratingReader(elementTypes, converter),
                                                                              new LegacyGetterFactory(tools,
                                                                                                      GetterFactory
                                                                                                          .Default)))),
                elementTypes,
                Activators.Default) {}

        public LegacyInstanceTypeConverter(ISpecification<TypeInfo> specification, IInstanceMembers members,
                                           IElementTypes elementTypes,
                                           IActivators activators)
            : base(
                specification, new InstanceBodyReader(members, elementTypes, activators),
                new TypeEmittingWriter(new InstanceBodyWriter(members))) {}
    }
}