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
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Sources;

namespace ExtendedXmlSerialization.Instructions.Write
{
    class InstanceBodySelector : IParameterizedSource<IContext, IInstruction>
    {
        readonly private static Func<ITypeDefinition, bool> Enumerable =
            IsEnumerableTypeSpecification.Default.IsSatisfiedBy;

        public static InstanceBodySelector Default { get; } = new InstanceBodySelector();
        InstanceBodySelector()
            : this(
                EmitCurrentInstanceInstruction.Default, EmitDictionaryInstruction.Default,
                EmitEnumerableInstruction.Default, EmitInstanceMembersInstruction.Default) {}

        private readonly IInstruction _primitive, _dictionary, _enumerable, _members;

        public InstanceBodySelector(IInstruction primitive, IInstruction dictionary, IInstruction enumerable,
                                    IInstruction members)
        {
            _primitive = primitive;
            _dictionary = dictionary;
            _enumerable = enumerable;
            _members = members;
        }

        public IInstruction Get(IContext parameter)
        {
            var instance = parameter.Instance;
            var type = instance.GetType();
            var definition = parameter.Definition;
            if (definition.IsPrimitive)
            {
                return _primitive;
            }

            if (definition.IsDictionary)
            {
                return _dictionary;
            }

            if (Enumerable(definition))
            {
                return _enumerable;
            }

            if (definition.IsObjectToSerialize)
            {
                return _members;
            }

            throw new InvalidOperationException($"Could not find instructions for type '{type}'");
        }
    }

    class EmitInstanceBodyInstruction : WriteInstructionBase
    {
        public static EmitInstanceBodyInstruction Default { get; } = new EmitInstanceBodyInstruction();
        EmitInstanceBodyInstruction() : this(InstanceBodySelector.Default) {}

        private readonly IParameterizedSource<IContext, IInstruction> _selector;

        public EmitInstanceBodyInstruction(IParameterizedSource<IContext, IInstruction> selector)
        {
            _selector = selector;
        }

        protected override void OnExecute(ISerialization services) => _selector.Get(services.Current).Execute(services);
    }

    class EmitInstanceBodyInstructionInstance : DeferredInstruction
    {
        public static EmitInstanceBodyInstructionInstance Default { get; } = new EmitInstanceBodyInstructionInstance();
        EmitInstanceBodyInstructionInstance() : base(() => EmitInstanceBodyInstruction.Default) {}
    }

    public struct MemberInstruction
    {
        public MemberInstruction(IMemberDefinition member, IInstruction instruction)
        {
            Member = member;
            Instruction = instruction;
        }

        public IMemberDefinition Member { get; }
        public IInstruction Instruction { get; }
    }

    public class MemberTemplate
    {
        public MemberTemplate(IImmutableList<IMemberDefinition> source, IImmutableList<MemberInstruction> templates)
        {
            Source = source;
            Templates = templates;
        }

        public IImmutableList<IMemberDefinition> Source { get; }
        public IImmutableList<MemberInstruction> Templates { get; }
    }

    public interface IMemberTemplates : IParameterizedSource<Type, MemberTemplate> {}

    class MemberTemplates : WeakCacheBase<Type, MemberTemplate>, IMemberTemplates
    {
        public static MemberTemplates Default { get; } = new MemberTemplates();
        MemberTemplates() : this(EmitMemberInstruction.Default.Accept) {}

        private readonly Func<IMemberDefinition, IInstruction> _select;

        public MemberTemplates(Func<IMemberDefinition, IInstruction> select)
        {
            _select = @select;
        }

        protected override MemberTemplate Callback(Type key)
        {
            var source = TypeDefinitionCache.GetDefinition(key).Members;
            var instructions = source.Select(_select);
            var templates =
                source.Zip(instructions, (context, instruction) => new MemberInstruction(context, instruction))
                      .ToImmutableList();
            var result = new MemberTemplate(source, templates);
            return result;
        }
    }
}