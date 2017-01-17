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
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Write
{
    class DictionaryBodyWriter : WriterBase<IDictionary>
    {
        private readonly IWriter _writer;

        public DictionaryBodyWriter(IWriter writer)
        {
            _writer = writer;
        }

        protected override void Write(IWriteContext context, IDictionary instance)
        {
            var container = ((IDictionaryElement) context.Element).Item;
            var element = context.New(container);
            foreach (DictionaryEntry entry in instance)
            {
                using (var child = element.Emit(container.DeclaredType))
                {
                    var k = child.New(container.Key);
                    var v = child.New(container.Value);

                    using (var sub = k.Emit(entry.Key.GetType().GetTypeInfo()))
                    {
                        _writer.Write(sub, entry.Key);
                    }

                    using (var sub = v.Emit(entry.Value.GetType().GetTypeInfo()))
                    {
                        _writer.Write(sub, entry.Value);
                    }
                }
            }
        }
    }
}