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
using System.Collections.Generic;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class DictionaryEntryReader : ReaderBase<IEnumerable>, IEnumeratingReader
    {
        readonly private static DictionaryPairTypesLocator Locator =
            new DictionaryPairTypesLocator(typeof(KeyValuePair<,>));

        private readonly ITypes _types;
        private readonly IReader _reader;
        private readonly IDictionaryPairTypesLocator _locator;

        public DictionaryEntryReader(ITypes types, IReader reader)
            : this(types, reader, Locator) {}

        public DictionaryEntryReader(ITypes types, IReader reader, IDictionaryPairTypesLocator locator)
        {
            _types = types;
            _reader = reader;
            _locator = locator;
        }

        public override IEnumerable Read(XElement element, Typed? hint = null) => Entries(element, hint);

        IEnumerable<DictionaryEntry> Entries(XContainer element, Type dictionaryType)
        {
            var pair = _locator.Get(dictionaryType);
            foreach (var child in element.Elements(LegacyNames.Item))
            {
                var key = Read(child, LegacyNames.Key, pair.KeyType);
                var value = Read(child, LegacyNames.Value, pair.ValueType);
                yield return new DictionaryEntry(key, value);
            }
        }

        object Read(XContainer element, XName name, Type type)
        {
            var child = element.Element(name);
            var result = _reader.Read(child, _types.Get(child) ?? type);
            return result;
        }
    }
}