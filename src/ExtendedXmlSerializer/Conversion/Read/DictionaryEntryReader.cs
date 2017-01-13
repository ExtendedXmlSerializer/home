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
using System.Collections.Generic;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class DictionaryEntryReader : ReaderBase<IEnumerable>, IEnumeratingReader
    {
        readonly private static DictionaryPairTypesLocator Locator = DictionaryPairTypesLocator.Default;

        private readonly IReader _reader;
        private readonly IDictionaryPairTypesLocator _locator;

        public DictionaryEntryReader(IReader reader) : this(reader, Locator) {}

        public DictionaryEntryReader(IReader reader, IDictionaryPairTypesLocator locator)
        {
            _reader = reader;
            _locator = locator;
        }

        public override IEnumerable Read(IReadContext context) => Entries(context);

        IEnumerable<DictionaryEntry> Entries(IReadContext context)
        {
            var pair = _locator.Get(context.Name.ReferencedType);
            foreach (var child in context.ChildrenOf(ItemProperty.Default))
            {
                var key = Read(child, new DictionaryKeyElement(pair.KeyType));
                var value = Read(child, new DictionaryKeyElement(pair.ValueType));
                yield return new DictionaryEntry(key, value);
            }
        }

        object Read(IReadContext context, IElement element) => _reader.Read(context.Member(element));
    }
}