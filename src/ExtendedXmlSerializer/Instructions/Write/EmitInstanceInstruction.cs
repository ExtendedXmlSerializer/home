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

using ExtendedXmlSerialization.Services.Write;

namespace ExtendedXmlSerialization.Instructions.Write
{
    class EmitInstanceInstruction : DecoratedWriteInstruction
    {
        private readonly IElementProvider _provider;

        public EmitInstanceInstruction(string name, IInstruction instruction)
            : this(new FixedNameProvider(name), instruction) {}

        public EmitInstanceInstruction(MemberContext member, IInstruction instruction)
            : this(new MemberInfoElementProvider(member), instruction) {}

        public EmitInstanceInstruction(IElementProvider provider, IInstruction instruction) : base(instruction)
        {
            _provider = provider;
        }

        protected override void OnExecute(IWriting services)
        {
            var element = _provider.Get(services, services.Current.Instance);
            services.Begin(element);
            base.OnExecute(services);
            services.EndElement();
        }
    }
}