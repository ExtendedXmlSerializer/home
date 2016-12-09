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

using System.Collections.Generic;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Services.Services;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.Plans.Write
{
    public class Plans : IPlans
    {
        private readonly IInstructionSpecification _specification;
        private readonly ITemplateElementProvider _provider;
        private readonly ISpecification<IWritingContext> _emitTypeSpecification;
        private readonly IInstruction _emitType;

        public Plans(IInstructionSpecification specification, ITemplateElementProvider provider,
                     ISpecification<IWritingContext> emitTypeSpecification, IInstruction emitType)
        {
            _specification = specification;
            _provider = provider;
            _emitTypeSpecification = emitTypeSpecification;
            _emitType = emitType;
        }

        public IEnumerable<IPlan> Get(IPlan parameter)
        {
            var factory = new MemberInstructionFactory(parameter, _specification);
            var emitType = new ConditionalInstruction<IWriting>(_emitTypeSpecification, _emitType);
            var members = new InstanceMembersWritePlan(parameter, emitType, _specification, factory);

            yield return PrimitiveWritePlan.Default;
            yield return new DictionaryWritePlan(parameter, members);
            yield return new EnumerableWritePlan(parameter, members, _provider);
            yield return new InstanceWritePlan(parameter, _emitType, _specification, factory);
            yield return new GeneralObjectWritePlan(parameter, _emitType);
        }
    }
}