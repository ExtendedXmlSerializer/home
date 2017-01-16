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
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class ListReader : ListReaderBase
    {
        private readonly IActivators _activators;
        private readonly IAddDelegates _add;

        public ListReader(IReader reader) : this(reader, Activators.Default, AddDelegates.Default) {}

        public ListReader(IReader reader, IActivators activators, IAddDelegates add) : base(reader)
        {
            _activators = activators;
            _add = add;
        }

        protected override object Create(IReadContext context, IEnumerable enumerable)
        {
            var type = context.Classification;
            var result = _activators.Activate<object>(type.AsType());
            var list = result as IList ?? new ListAdapter(result, _add.Get(type));
            foreach (var item in enumerable)
            {
                list.Add(item);
            }
            return result;
        }
    }
}