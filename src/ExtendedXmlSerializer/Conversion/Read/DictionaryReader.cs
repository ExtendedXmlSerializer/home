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
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class DictionaryReader : ListReaderBase
    {
        private readonly IActivators _activators;

        public DictionaryReader(IElementTypes elementTypes, IReader reader) : this(elementTypes, reader, Activators.Default) {}

        public DictionaryReader(IElementTypes elementTypes, IReader reader, IActivators activators)
            : base(new DictionaryEntryReader(elementTypes, reader), EnumerableTypingsStore.Default.Get(elementTypes))
        {
            _activators = activators;
        }

        protected override object Create(IEnumerable enumerable, EnumerableTyping typing)
        {
            var result = _activators.Activate<IDictionary>(typing.Type);
            foreach (DictionaryEntry item in enumerable)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
    }
}