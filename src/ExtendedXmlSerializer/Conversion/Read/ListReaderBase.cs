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
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public abstract class ListReaderBase : ReaderBase
    {
        private readonly ITypes _types;
        private readonly IEnumeratingReader _reader;
        private readonly IElementTypeLocator _locator;

        protected ListReaderBase(ITypes types, IReader reader)
            : this(types, new EnumeratingReader(types, reader), ElementTypeLocator.Default) {}

        protected ListReaderBase(ITypes types, IEnumeratingReader reader, IElementTypeLocator locator)
        {
            _types = types;
            _reader = reader;
            _locator = locator;
        }

        public sealed override object Read(XElement element, Typed? hint = null)
        {
            var type = hint ?? _types.Get(element);
            var elementType = _locator.Locate(type);
            var enumerable = _reader.Read(element, elementType);
            var result = Create(type, enumerable, elementType);
            return result;
        }

        protected abstract object Create(Type listType, IEnumerable enumerable, Type elementType);
    }
}