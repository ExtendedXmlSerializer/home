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
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class EnumeratingReader : ReaderBase<IEnumerable>, IEnumeratingReader
    {
        private readonly ITypes _types;
        private readonly IReader _reader;
        private readonly IEnumerableTypings _typings;

        public EnumeratingReader(ITypes types, IReader reader)
            : this(types, reader, EnumerableTypingsStore.Default.Get(types)) {}

        public EnumeratingReader(ITypes types, IReader reader, IEnumerableTypings typings)
        {
            _types = types;
            _reader = reader;
            _typings = typings;
        }

        public override IEnumerable Read(XElement element)
        {
            var elementType = _typings.Get(element).ElementType;
            foreach (var child in element.Elements())
            {
                var initialized = _types.Initialized(child, elementType);
                var item = _reader.Read(initialized);
                yield return item;
            }
        }
    }
}