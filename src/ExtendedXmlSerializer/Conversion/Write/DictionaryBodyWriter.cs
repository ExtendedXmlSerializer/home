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
using System.Reflection;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Write
{
    class DictionaryBodyWriter : WriterBase<IDictionary>
    {
        private readonly IWriter _writer;
        private readonly IDictionaryPairTypesLocator _locator;

        public DictionaryBodyWriter(IWriter item)
            : this(new DictionaryEntryWriter(item), DictionaryPairTypesLocator.Default) {}

        DictionaryBodyWriter(IWriter writer, IDictionaryPairTypesLocator locator)
        {
            _writer = writer;
            _locator = locator;
        }

        protected override void Write(IWriteContext context, IDictionary instance)
        {
            var pair = _locator.Get(instance.GetType().GetTypeInfo());
            var element = new DictionaryElement(ItemProperty.Default, pair.KeyType, pair.ValueType);

            foreach (DictionaryEntry entry in instance)
            {
                using (var child = context.Start(element))
                {
                    _writer.Write(child, entry);
                }
            }
        }
    }
}