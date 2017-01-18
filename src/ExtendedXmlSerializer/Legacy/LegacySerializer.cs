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

using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Legacy
{
    sealed class LegacySerializer : Serializer
    {
        private readonly IElements _selector;

        public LegacySerializer(IElements selector, IConverter converter) : base(converter)
        {
            _selector = selector;
        }

        protected override IWriteContext CreateWriteContext(XmlWriter writer, TypeInfo type)
            => new XmlWriteContext(_selector, LegacyNamespaces.Default, writer, new Root(_selector.Build(type)),
                                   new WriteReferences(), writer);

        protected override IReadContext CreateContext(Stream stream)
        {
            var text = new StreamReader(stream).ReadToEnd();
            var document = XDocument.Parse(text);
            var result = new LegacyXmlReadContext(document.Root, SerializerTypes.Default.Get(this));
            return result;
        }
    }

    // HACK: This is to get working in current state.  This will be removed at some point.
    sealed class SerializerTypes : WeakCache<ISerializer, TypeInfo>
    {
        public static SerializerTypes Default { get; } = new SerializerTypes();
        SerializerTypes() : base(x => null) {}
    }
}