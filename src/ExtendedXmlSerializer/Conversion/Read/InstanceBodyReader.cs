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

using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    class InstanceBodyReader : ReaderBase
    {
        private readonly IMemberConverterSelector _selector;
        private readonly IActivators _activators;

        public InstanceBodyReader(IMemberConverterSelector selector) : this(selector, Activators.Default) {}

        public InstanceBodyReader(IMemberConverterSelector selector, IActivators activators)
        {
            _selector = selector;
            _activators = activators;
        }

        public override object Read(IReadContext context)
        {
            var result = Activate(context);
            var element = context.Current as IMemberedElement;
            if (element != null)
            {
                var members = element.Members;
                foreach (var child in context)
                {
                    var member = members.Get(child.DisplayName);
                    if (member != null)
                    {
                        var reader = _selector.Get(member);
                        var value = reader.Read(child);
                        member.Assign(result, value);
                    }
                }
            }
            return result;
        }

        protected virtual object Activate(IReadContext context)
            => _activators.Activate<object>(context.Classification.AsType());
    }
}