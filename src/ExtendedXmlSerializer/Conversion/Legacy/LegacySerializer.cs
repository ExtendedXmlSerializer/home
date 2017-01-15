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
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    sealed class LegacySerializer : Serializer
    {
        readonly private static TypeInfo Type = typeof(LegacySerializer).GetTypeInfo();

        private readonly IElementSelector _selector;
        private readonly TypeInfo _type;

        public LegacySerializer(IConverter converter, IElementSelector selector) : this(converter, selector, Type) {}

        public LegacySerializer(IConverter converter, IElementSelector selector, TypeInfo type) : base(converter)
        {
            _selector = selector;
            _type = type;
        }

        protected override IWriteContext CreateWriteContext(XmlWriter writer)
            => new LegacyXmlWriteContext(_selector, writer);

        protected override IReadContext CreateContext(Stream stream)
        {
            var text = new StreamReader(stream).ReadToEnd();
            var document = XDocument.Parse(text);
            var result = new LegacyXmlReadContext(document.Root, _type);
            return result;
        }
    }
}