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
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Instructions.Write;

namespace ExtendedXmlSerialization.Plans.Write
{
    class EnumerableWritePlan : ConditionalPlanBase
    {
        private readonly IPlan _plan;
        private readonly IPlan _members;
        private readonly ITemplateElementProvider _provider;

        public EnumerableWritePlan(IPlan plan, IPlan members, ITemplateElementProvider provider)
            : this(IsEnumerableTypeSpecification.Default.IsSatisfiedBy, plan, members, provider) {}

        public EnumerableWritePlan(Func<Type, bool> specification, IPlan plan, IPlan members,
                                   ITemplateElementProvider provider) : base(specification)
        {
            _plan = plan;
            _members = members;
            _provider = provider;
        }

        protected override IInstruction Plan(Type type)
        {
            var elementType = ElementTypeLocator.Default.Locate(type);
            var template = new EmitInstanceInstruction(_provider.For(elementType), _plan.For(elementType));

            var result = new CompositeInstruction(
                             _members.For(type),
                             new EmitEnumerableInstruction(template)
                         );
            return result;
        }
    }
}