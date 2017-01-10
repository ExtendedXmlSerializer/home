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

using System.Collections;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public abstract class ListReaderBase : ReaderBase
    {
        private readonly IEnumeratingReader _reader;
        private readonly IEnumerableTypings _typings;

        protected ListReaderBase(IElementTypes elementTypes, IReader reader)
            : this(elementTypes, reader, EnumerableTypingsStore.Default.Get(elementTypes)) {}

        protected ListReaderBase(IElementTypes elementTypes, IReader reader, IEnumerableTypings typings)
            : this(new EnumeratingReader(elementTypes, reader, typings), typings) {}

        protected ListReaderBase(IEnumeratingReader reader, IEnumerableTypings typings)
        {
            _reader = reader;
            _typings = typings;
        }

        public sealed override object Read(XElement element)
        {
            var typing = _typings.Get(element);
            var enumerable = _reader.Read(element);
            var result = Create(enumerable, typing);
            return result;
        }

        protected abstract object Create(IEnumerable enumerable, EnumerableTyping typing);
    }
}