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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Instructions.Write;
using ExtendedXmlSerialization.Plans;
using ExtendedXmlSerialization.Plans.Write;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Sources;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public class Serializer : ISerializer
    {
        private readonly IPlan _plan;
        private readonly IWritingFactory _factory;

        public Serializer(IPlan plan, IWritingFactory factory)
        {
            _plan = plan;
            _factory = factory;
        }

        public void Serialize(Stream stream, object instance)
        {
            using (var writing = _factory.Get(stream))
            {
                using (writing.Start(instance))
                {
                    EmitRootInstruction.Default.Execute(writing);
                }
            }
        }
    }

    class InstanceBodySelector : IParameterizedSource<IWriting, IInstruction>
    {
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

        public IInstruction Get(IWriting parameter)
        {
            var instance = parameter.Current.Instance;
            var type = instance.GetType();
            var definition = TypeDefinitionCache.GetDefinition(type);
            if (definition.IsPrimitive)
            {
                return _primitive;
            }

            if (instance is IDictionary)
            {
                return _dictionary;
            }

            if (Arrays.Default.Is(instance))
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

        private readonly IParameterizedSource<IWriting, IInstruction> _selector;

        public EmitInstanceBodyInstruction(IParameterizedSource<IWriting, IInstruction> selector)
        {
            _selector = selector;
        }

        protected override void OnExecute(IWriting services) => _selector.Get(services).Execute(services);
    }

    class EmitInstanceMembersInstruction : WriteInstructionBase
    {
        public static EmitInstanceMembersInstruction Default { get; } = new EmitInstanceMembersInstruction();
        EmitInstanceMembersInstruction() : this(EmitTypeInstruction.Default) {}

        private readonly Func<MemberContext, bool> _specification;
        private readonly IInstruction _type, _property, _content;

        public EmitInstanceMembersInstruction(IInstruction type)
            : this(NeverSpecification<MemberContext>.Default.IsSatisfiedBy, type, EmitMemberAsTextInstruction.Default, EmitMemberInstruction.Default) {}

        public EmitInstanceMembersInstruction(
            Func<MemberContext, bool> specification,
            IInstruction type,
            IInstruction property, IInstruction content)
        {
            _specification = specification;
            _type = type;
            _property = property;
            _content = content;
        }

        protected override void OnExecute(IWriting services)
        {
            var contexts = MemberContexts.Default.Get(services.Current.Instance);
            using (services.New(contexts))
            {
                _type.Execute(services);

                var categories = contexts.GroupBy(_specification).ToArray();
                for (int index = 0; index < categories.Length; index++)
                {
                    var category = categories[index];
                    var instruction = category.Key ? _property : _content;
                    var array = category.ToArray();
                    for (int i = 0; i < array.Length; i++)
                    {
                        using (services.New(array[i]))
                        {
                            instruction.Execute(services);
                        }
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

        public void Execute(IServiceProvider services) => _source.Value.Execute(services);
    }

    class EmitInstanceBodyInstructionInstance : DeferredInstruction
    {
        public static EmitInstanceBodyInstructionInstance Default { get; } = new EmitInstanceBodyInstructionInstance();
        EmitInstanceBodyInstructionInstance() : base(() => EmitInstanceBodyInstruction.Default) {}
    }

    class StartNewContextFromMemberValueInstruction : WriteInstructionBase
    {
        public static StartNewContextFromMemberValueInstruction Default { get; } = new StartNewContextFromMemberValueInstruction();
        StartNewContextFromMemberValueInstruction() : this(EmitInstanceBodyInstructionInstance.Default) {}

        private readonly IInstruction _instruction;
        
        public StartNewContextFromMemberValueInstruction(IInstruction instruction)
        {
            _instruction = instruction;
        }

        protected override void OnExecute(IWriting services)
        {
            using (services.New(services.Current.Member?.Value))
            {
                _instruction.Execute(services);
            }
        }
    }

    class EmitMemberInstruction : EmitInstanceInstruction
    {
        public static EmitMemberInstruction Default { get; } = new EmitMemberInstruction();
        EmitMemberInstruction() : this(StartNewContextFromMemberValueInstruction.Default) {}

        public EmitMemberInstruction(IInstruction body) : base(Source.Instance.Get, body) {}

        class Source : IParameterizedSource<IWriting, IElement>
        {
            public static Source Instance { get; } = new Source();
            Source() {}

            public IElement Get(IWriting parameter)
            {
                var context = parameter.Current.Member.Value;
                var result = new Element(parameter.Get(context.Metadata.DeclaringType), context.DisplayName);
                return result;
            }
        }
    }

    class EmitInstanceInstruction : WriteInstructionBase
    {
        private readonly Func<IWriting, IElement> _element;
        private readonly IInstruction _body;

        public EmitInstanceInstruction(Func<IWriting, IElement> element) : this(element, EmitInstanceBodyInstructionInstance.Default) {}

        public EmitInstanceInstruction(Func<IWriting, IElement> element, IInstruction body)
        {
            _element = element;
            _body = body;
        }

        protected override void OnExecute(IWriting services)
        {
            var element = _element(services);
            services.Begin(element);
            _body.Execute(services);
            services.EndElement();
        }
    }

    class EmitDictionaryInstruction : WriteInstructionBase<IDictionary>
    {
        readonly private static Type Key = typeof(EmitDictionaryInstruction);
        public static EmitDictionaryInstruction Default { get; } = new EmitDictionaryInstruction();
        EmitDictionaryInstruction()
            : this(new EmitInstanceMembersInstruction(DefaultEmitTypeForInstanceInstruction.Default)) {}

        private readonly IInstruction _members;

        public EmitDictionaryInstruction(IInstruction members)
        {
            _members = members;
        }
        
        protected override void Execute(IWriting services, IDictionary instance)
        {
            _members.Execute(services);

            var key = new EmitInstanceInstruction(new DictionaryKeyElement(services.Get(Key)).Accept);
            var value = new EmitInstanceInstruction(new DictionaryValueElement(services.Get(Key)).Accept);
            foreach (DictionaryEntry item in instance)
            {
                using (services.NewInstance(item))
                {
                    services.Begin(new DictionaryItemElement(services.Get(Key)));
                    using (services.New(item.Key))
                    {
                        key.Execute(services);
                    }

                    using (services.New(item.Value))
                    {
                        value.Execute(services);
                    }
                    services.EndElement();
                }
            }
        }
    }

    class EmitEnumerableInstruction : WriteInstructionBase
    {
        readonly private static EmitInstanceInstruction Instance = new EmitInstanceInstruction(Source.DefaultInstance.Get);
        public static EmitEnumerableInstruction Default { get; } = new EmitEnumerableInstruction();
        EmitEnumerableInstruction()
            : this(new EmitInstanceMembersInstruction(DefaultEmitTypeForInstanceInstruction.Default)) {}

        private readonly IInstruction _members;
        private readonly IInstruction _instance;

        public EmitEnumerableInstruction(IInstruction members) : this(members, Instance) {}

        public EmitEnumerableInstruction(IInstruction members, IInstruction instance)
        {
            _members = members;
            _instance = instance;
        }

        protected override void OnExecute(IWriting services)
        {
            _members.Execute(services);

            var instance = services.Current.Instance;
            foreach (var item in Arrays.Default.AsArray(instance))
            {
                using (services.New(item))
                {
                    _instance.Execute(services);
                }
            }
        }

        sealed class Source : IParameterizedSource<IWriting, IElement>
        {
            public static Source DefaultInstance { get; } = new Source();
            Source() {}

            public IElement Get(IWriting parameter)
            {
                var instance = parameter.GetArrayContext()?.Instance;
                var type = instance.GetType();
                var elementType = ElementTypeLocator.Default.Locate(type);

                var target = elementType.GetTypeInfo().IsInterface
                    ? instance.GetType()
                    : elementType;
                
                var result = new Element(parameter.Get(instance), TypeDefinitionCache.GetDefinition(target).Name);
                return result;

            }
        }
    }

    class EmitRootInstruction : WriteInstructionBase
    {
        private readonly IInstruction _body;

        public static EmitRootInstruction Default { get; } = new EmitRootInstruction();
        EmitRootInstruction() : this(EmitInstanceBodyInstruction.Default) {}

        public EmitRootInstruction(IInstruction body)
        {
            _body = body;
        }

        protected override void OnExecute(IWriting services)
        {
            var root = services.Current.Root;
            using (services.New(root))
            {
                var element = new RootElement(services.Get(root), root);
                services.Start(element);
                _body.Execute(services);
                services.EndElement();
            }
        }
    }
}