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

using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    class InstanceBodyReader : ReaderBase
    {
        private readonly IMembers _members;
        private readonly IConverterSelector _selector;
        private readonly IActivators _activators;

        public InstanceBodyReader(IMembers members, IConverterSelector selector)
            : this(members, selector, Activators.Default) {}

        public InstanceBodyReader(IMembers members, IConverterSelector selector, IActivators activators)
        {
            _members = members;
            _selector = selector;
            _activators = activators;
        }

        public override object Read(IReadContext context)
        {
            var result = Activate(context);
            OnRead(context, result);
            return result;
        }

        protected virtual object Activate(IReadContext context)
            => _activators.Activate<object>(context.Name.ReferencedType.AsType());

        protected virtual void OnRead(IReadContext context, object result)
        {
            var members = _members.Get(context.Name.ReferencedType);
            foreach (var child in context)
            {
                var member = members.Get(context.Name.Name);
                if (member != null)
                {
                    var converter = _selector.Get(member);
                    member.Assign(result, converter.Read(child));
                    
                }
            }
        }
    }
}