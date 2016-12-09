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

using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Instructions.Write;
using ExtendedXmlSerialization.Services.Services;

namespace ExtendedXmlSerialization.Plans.Write
{
    class MemberInstructionFactory : IMemberInstructionFactory
    {
        readonly private static IInstruction Property =
            new StartNewMemberValueContextInstruction(EmitMemberAsTextInstruction.Default);

        private readonly IPlan _plan;
        private readonly IInstructionSpecification _specification;
        readonly private IInstruction _property;

        public MemberInstructionFactory(IPlan plan, IInstructionSpecification specification)
            : this(plan, specification, Property) {}

        public MemberInstructionFactory(IPlan plan, IInstructionSpecification specification, IInstruction property)
        {
            _plan = plan;
            _specification = specification;
            _property = property;
        }

        bool Specification<T>(T parameter) => _specification.IsSatisfiedBy(parameter);

        IInstruction Content(MemberContext member) =>
            new EmitInstanceInstruction(
                member,
                new StartNewMemberValueContextInstruction(
                    new StartNewContextFromMemberValueInstruction(_plan.For(member.MemberType))
                )
            );

        public IMemberInstruction Get(MemberContext parameter)
        {
            var property = Specification(parameter);
            var content = property ? _property : Content(parameter);
            var context = new StartNewMemberContextInstruction(parameter.Metadata, content);
            var result = property
                ? (IMemberInstruction) new EmitMemberAsPropertyInstruction(context)
                : new EmitMemberAsContentInstruction(context);
            return result;
        }
    }
}