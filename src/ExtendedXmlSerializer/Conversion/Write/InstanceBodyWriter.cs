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

using System.Reflection;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.Conversion.Write
{
    class InstanceBodyWriter : WriterBase
    {
        private readonly IElementSelector _elements;
        private readonly IMemberConverterSelector _selector;

        public InstanceBodyWriter(IMemberConverterSelector selector) : this(Elements.Default, selector) {}

        public InstanceBodyWriter(IElementSelector elements, IMemberConverterSelector selector)
        {
            _elements = elements;
            _selector = selector;
        }

        public override void Write(IWriteContext context, object instance)
        {
            var type = instance.GetType().GetTypeInfo();
            var element = Equals(type, context.Classification) ? context.Element : _elements.Get(type);
            var members = ((IMemberedElement) element).Members;
            foreach (var member in members)
            {
                _selector.Get(member).Write(context, member.Get(instance));
            }
        }
    }
}