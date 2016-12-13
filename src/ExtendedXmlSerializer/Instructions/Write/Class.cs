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
    class InstanceBodySelector : IParameterizedSource<ISerialization, IInstruction>
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

        public IInstruction Get(ISerialization parameter)
        {
            var instance = parameter.Current.Instance;
            var type = instance.GetType();
            var definition = parameter.Current.Definition;
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

        private readonly IParameterizedSource<ISerialization, IInstruction> _selector;

        public EmitInstanceBodyInstruction(IParameterizedSource<ISerialization, IInstruction> selector)
        {
            _selector = selector;
        }

        protected override void OnExecute(ISerialization services) => _selector.Get(services).Execute(services);
    }

    class EmitInstanceBodyInstructionInstance : DeferredInstruction
    {
        public static EmitInstanceBodyInstructionInstance Default { get; } = new EmitInstanceBodyInstructionInstance();
        EmitInstanceBodyInstructionInstance() : base(() => EmitInstanceBodyInstruction.Default) {}
    }

    public struct MemberInstruction
    {
        public MemberInstruction(MemberContext member, IInstruction instruction)
        {
            Member = member;
            Instruction = instruction;
        }

        public MemberContext Member { get; }
        public IInstruction Instruction { get; }
    }

    public class MemberTemplate
    {
        public MemberTemplate(IImmutableList<MemberContext> source, IImmutableList<MemberInstruction> templates)
        {
            Source = source;
            Templates = templates;
        }

        public IImmutableList<MemberContext> Source { get; }
        public IImmutableList<MemberInstruction> Templates { get; }
    }

    public interface IMemberTemplates : IParameterizedSource<Type, MemberTemplate> {}

    class MemberTemplates : WeakCacheBase<Type, MemberTemplate>, IMemberTemplates
    {
        public static MemberTemplates Default { get; } = new MemberTemplates();
        MemberTemplates() : this(EmitMemberInstruction.Default.Accept) {}

        private readonly Func<MemberContext, IInstruction> _select;

        public MemberTemplates(Func<MemberContext, IInstruction> select)
        {
            _select = @select;
        }

        protected override MemberTemplate Callback(Type key)
        {
            var source = SerializableMembers.Default.Get(key);
            var instructions = source.Select(_select);
            var templates =
                source.Zip(instructions, (context, instruction) => new MemberInstruction(context, instruction))
                      .ToImmutableList();
            var result = new MemberTemplate(source, templates);
            return result;
        }
    }

    class DefaultEmitInstanceMembersInstruction : EmitInstanceMembersInstruction
    {
        public new static DefaultEmitInstanceMembersInstruction Default { get; } =
            new DefaultEmitInstanceMembersInstruction();

        DefaultEmitInstanceMembersInstruction()
            : base(DefaultEmitTypeForInstanceInstruction.Default, MemberTemplates.Default) {}
    }

    class EmitInstanceMembersInstruction : WriteInstructionBase
    {
        public static EmitInstanceMembersInstruction Default { get; } = new EmitInstanceMembersInstruction();
        EmitInstanceMembersInstruction() : this(EmitTypeInstruction.Default, MemberTemplates.Default) {}

        private readonly IInstruction _type;
        private readonly IMemberTemplates _templates;

        public EmitInstanceMembersInstruction(IInstruction type, IMemberTemplates templates)
        {
            _type = type;
            _templates = templates;
        }

        protected override void OnExecute(ISerialization services)
        {
            var templates = _templates.Get(services.Current.Definition.Type);
            using (services.New(templates.Source))
            {
                _type.Execute(services);

                var items = templates.Templates;
                var count = items.Count;
                for (var i = 0; i < count; i++)
                {
                    var item = items[i];
                    using (services.New(item.Member))
                    {
                        item.Instruction.Execute(services);
                    }
                }
            }
        }
    }

    class DeferredInstruction : IInstruction
    {
        private readonly Lazy<IInstruction> _source;

        public DeferredInstruction(Func<IInstruction> source) : this(new Lazy<IInstruction>(source)) {}

        public DeferredInstruction(Lazy<IInstruction> source)
        {
            _source = source;
        }

        public void Execute(IProcess services) => _source.Value.Execute(services);
    }

    class EmitMemberInstruction : EmitInstanceInstruction
    {
        public static EmitMemberInstruction Default { get; } = new EmitMemberInstruction();

        EmitMemberInstruction()
            : base(context => context.Value(), MemberElementInformation.Instance,
                   EmitInstanceBodyInstructionInstance.Default) {}
    }

    class EmitInstanceInstruction : EmitInstanceInstructionBase
    {
        private readonly Func<IWriteContext, object> _instance;

        public EmitInstanceInstruction(Func<IWriteContext, object> instance, IElementInformation information,
                                       IInstruction body) : base(information, body)
        {
            _instance = instance;
        }

        protected override object GetInstance(IWriteContext context) => _instance(context);
    }

    abstract class EmitInstanceInstructionBase : WriteInstructionBase
    {
        private readonly IElementInformation _information;
        private readonly IInstruction _body;

        protected EmitInstanceInstructionBase(IElementInformation information, IInstruction body)
        {
            _information = information;
            _body = body;
        }

        protected abstract object GetInstance(IWriteContext context);

        protected override void OnExecute(ISerialization services)
        {
            var instance = GetInstance(services.Current);
            using (services.New(instance, _information))
            {
                _body.Execute(services);
            }
        }
    }

    class EmitDictionaryInstruction : WriteInstructionBase<IDictionary>
    {
        public static EmitDictionaryInstruction Default { get; } = new EmitDictionaryInstruction();

        EmitDictionaryInstruction()
            : this(DefaultEmitInstanceMembersInstruction.Default, EmitInstanceBodyInstructionInstance.Default) {}

        private readonly IInstruction _members;
        private readonly IInstruction _body;

        public EmitDictionaryInstruction(IInstruction members, IInstruction body)
        {
            _members = members;
            _body = body;
        }

        protected override void Execute(ISerialization services, IDictionary instance)
        {
            _members.Execute(services);

            foreach (DictionaryEntry item in instance)
            {
                using (services.NewInstance(item, DictionaryEntryElementInformation.Default))
                {
                    using (services.New(item.Key, DictionaryKeyElementInformation.Default))
                    {
                        _body.Execute(services);
                    }

                    using (services.New(item.Value, DictionaryValueElementInformation.Default))
                    {
                        _body.Execute(services);
                    }
                }
            }
        }
    }

    class EmitEnumerableInstruction : WriteInstructionBase
    {
        public static EmitEnumerableInstruction Default { get; } = new EmitEnumerableInstruction();

        EmitEnumerableInstruction()
            : this(
                DefaultEmitInstanceMembersInstruction.Default,
                EmitInstanceBodyInstructionInstance.Default, ItemElements.DefaultElement.Get) {}

        private readonly IInstruction _members;
        private readonly IInstruction _body;
        private readonly Func<ITypeDefinition, IElementInformation> _elements;

        public EmitEnumerableInstruction(IInstruction members, IInstruction body,
                                         Func<ITypeDefinition, IElementInformation> elements)
        {
            _members = members;
            _body = body;
            _elements = elements;
        }

        protected override void OnExecute(ISerialization services)
        {
            _members.Execute(services);

            var current = services.Current;
            var information = _elements(current.Definition);
            var array = Arrays.Default.AsArray(current.Instance);
            for (var i = 0; i < array.Length; i++)
            {
                using (services.New(array.GetValue(i), information))
                {
                    _body.Execute(services);
                }
            }
        }
    }

    class EmitRootInstruction : EmitInstanceInstructionBase
    {
        private readonly object _instance;

        public EmitRootInstruction(object instance) : this(instance, EmitInstanceBodyInstruction.Default) {}

        public EmitRootInstruction(object instance, IInstruction body) : base(RootElementInformation.Default, body)
        {
            _instance = instance;
        }

        protected override object GetInstance(IWriteContext context) => _instance;
    }
}