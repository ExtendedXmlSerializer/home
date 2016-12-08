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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Plans;
using ExtendedXmlSerialization.Write.Instructions;
using ExtendedXmlSerialization.Write.Services;

namespace ExtendedXmlSerialization.Write.Plans
{
    class InstanceMembersWritePlan : IPlan
    {
        readonly private static Func<Type, IImmutableList<MemberInfo>> Members = SerializableMembers.Default.Get;
        private readonly IInstruction _attachedProperties;
        private readonly IInstruction _emitType;
        private readonly Func<Type, IImmutableList<MemberInfo>> _members;
        private readonly Func<MemberInfo, bool> _deferred;
        private readonly Func<MemberContext, IMemberInstruction> _factory;

        public InstanceMembersWritePlan(IPlan primary, IInstruction emitType, IInstructionSpecification specification,
                                        IMemberInstructionFactory factory)
            : this(
                new EmitAttachedPropertiesInstruction(primary, specification.IsSatisfiedBy), emitType, factory.Get,
                specification.Defer, Members) {}

        public InstanceMembersWritePlan(
            IInstruction attachedProperties, IInstruction emitType, Func<MemberContext, IMemberInstruction> factory,
            Func<MemberInfo, bool> deferred, Func<Type, IImmutableList<MemberInfo>> members)
        {
            _attachedProperties = attachedProperties;
            _emitType = emitType;
            _deferred = deferred;
            _members = members;
            _factory = factory;
        }

        public IInstruction For(Type type)
        {
            var members = _members(type);
            var deferred = members.Where(_deferred).ToImmutableList();
            var instructions =
                members.Except(deferred).Select(info => new MemberContext(info)).Select(_factory).ToArray();
            var properties = instructions.OfType<IPropertyInstruction>().ToImmutableList();
            var contents = instructions.Except(properties).ToImmutableList();
            var body = new CompositeInstruction(
                           _emitType,
                           new CompositeInstruction(properties),
                           new EmitDifferentiatingMembersInstruction(type, _factory,
                                                                     new CompositeInstruction(
                                                                         new EmitDeferredMembersInstruction(deferred,
                                                                                                            _factory,
                                                                                                            _attachedProperties),
                                                                         new CompositeInstruction(contents)
                                                                     )
                           )
                       );

            var result = new StartNewMembersContextInstruction(members, body);
            return result;
        }
    }
}