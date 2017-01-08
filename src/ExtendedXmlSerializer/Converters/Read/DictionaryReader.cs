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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Converters.Read
{
    public class DictionaryReader : ListReaderBase
    {
        private readonly IActivators _activators;

        public DictionaryReader(ITypes types, IReader reader)
            : this(types, reader, Activators.Default, ElementTypeLocator.Default) {}

        public DictionaryReader(ITypes types, IReader reader, IActivators activators, IElementTypeLocator locator)
            : base(types, new DictionaryEntryReader(types, reader), locator)
        {
            _activators = activators;
        }

        protected override object Create(Type listType, IEnumerable enumerable, Type elementType)
        {
            var result = _activators.Activate<IDictionary>(new Typed(listType));
            foreach (DictionaryEntry item in enumerable)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
    }
}