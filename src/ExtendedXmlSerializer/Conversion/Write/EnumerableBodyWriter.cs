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

namespace ExtendedXmlSerialization.Conversion.Write
{
    public class EnumerableBodyWriter : WriterBase<IEnumerable>
    {
        private readonly IWriter _writer;

        public EnumerableBodyWriter(IWriter itemWriter) : this(Elements.Default, itemWriter) {}

        public EnumerableBodyWriter(IElements elements, IWriter itemWriter)
            : this(new ElementWriter(elements.Get, itemWriter)) {}

        EnumerableBodyWriter(ElementWriter writer)
        {
            _writer = writer;
        }

        protected override void Write(IWriteContext context, IEnumerable instance)
        {
            foreach (var item in instance)
            {
                _writer.Write(context, item);
            }
        }
    }
}