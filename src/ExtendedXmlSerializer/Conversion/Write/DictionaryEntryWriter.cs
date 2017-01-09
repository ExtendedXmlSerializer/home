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
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Write
{
    sealed class DictionaryEntryWriter : WriterBase<DictionaryEntry>
    {
        readonly private static Func<TypeInfo, XName>
            Key = LegacyNames.Key.Accept,
            Value = LegacyNames.Value.Accept;

        private readonly IWriter _key;
        private readonly IWriter _value;

        public DictionaryEntryWriter(IWriter writer)
            : this(new ElementWriter(Key, writer), new ElementWriter(Value, writer)) {}

        public DictionaryEntryWriter(IWriter key, IWriter value)
        {
            _key = key;
            _value = value;
        }

        protected override void Write(XmlWriter writer, DictionaryEntry instance)
        {
            _key.Write(writer, instance.Key);
            _value.Write(writer, instance.Value);
        }
    }
}