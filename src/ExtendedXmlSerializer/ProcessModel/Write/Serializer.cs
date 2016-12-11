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
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Instructions.Write;
using ExtendedXmlSerialization.Plans;
using ExtendedXmlSerialization.Plans.Write;
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
                    /*var instruction = Instructions.Default.Get(instance);
                    instruction.Execute(writing);*/
                    var instructions = Instructions.Default.Get(instance);// _plan.For(instance.GetType());
                    foreach (var instruction in instructions)
                    {
                        instruction.Execute(writing);
                    }
                }
            }
        }
    }

    public interface IInstructions : IParameterizedSource<object, IImmutableList<IInstruction>> {}

    class Instructions : /*WeakCacheBase<object, IImmutableList<IInstruction>>,*/ IInstructions
    {
        private readonly Stack<IDisposable> _contexts = new Stack<IDisposable>();
        private readonly Func<MemberContext, bool> _specification;
        private readonly IInstruction _emitType, _membersType, _endContext;
        private readonly ITemplateElementProvider _provider;

        public static Instructions Default { get; } = new Instructions();
        Instructions()
            : this(
                NeverSpecification<MemberContext>.Default.IsSatisfiedBy, EmitTypeInstruction.Default,
                DefaultEmitTypeForInstanceInstruction.Default,
                DefaultTemplateElementProvider.Default) {}

        public Instructions(Func<MemberContext, bool> specification, IInstruction emitType,
                            IInstruction membersType,
                            ITemplateElementProvider provider)
        {
            _specification = specification;
            _emitType = emitType;
            _membersType = membersType;
            _provider = provider;
            _endContext = new EndContextInstruction(_contexts);
        }

        public IImmutableList<IInstruction> Get(object key) => Yield(key).ToImmutableList();

        IEnumerable<IInstruction> Yield(object instance)
        {
            yield return new StartContextInstruction(_contexts, context => context.New(context.Current.Root));
            yield return StartRootInstruction.Default;
            foreach (var instruction in SelectBody(instance))
            {
                yield return instruction;
            }
            yield return EndCurrentElementInstruction.Default;
            yield return _endContext;
        }

        IEnumerable<IInstruction> InstanceMembers(object instance)
        {
            yield return new StartContextInstruction(_contexts, context => context.New(SerializableMembers.Default.Get(context.Current.Instance.GetType())));
            foreach (var instruction in YieldMembers(instance))
            {
                yield return instruction;
            }
            yield return _endContext;
        }

        IEnumerable<IInstruction> SelectBody(object instance)
        {
            var type = instance.GetType();
            var definition = TypeDefinitionCache.GetDefinition(type);
            if (definition.IsPrimitive)
            {
                return Primitive;
            }

            var dictionary = instance as IDictionary;
            if (dictionary != null)
            {
                if (definition.GenericArguments.Length != 2)
                {
                    throw new InvalidOperationException(
                              $"Attempted to write type '{type}' as a dictionary, but it does not have enough generic arguments.");
                }
                return Dictionary(dictionary);
            }

            if (Arrays.Default.Is(instance))
            {
                return Enumerable(Arrays.Default.AsArray(instance));
            }

            if (definition.IsObjectToSerialize)
            {
                return InstanceMembers(instance);
            }

            throw new InvalidOperationException($"Could not find instructions for type '{type}'");
        }

        /*private IInstruction GetEnumerableBody(IEnumerable instance)
        {
            var result = new CompositeInstruction(NewEnumerableBody(instance).ToArray());
            return result;
        }*/

        private IEnumerable<IInstruction> Enumerable(IEnumerable instance)
        {
            foreach (var instruction in YieldMembers(instance, _membersType))
            {
                yield return instruction;
            }

            var type = instance.GetType();
            var elementType = ElementTypeLocator.Default.Locate(type);
            var provider = _provider.For(elementType);

            foreach (var item in Arrays.Default.AsArray(instance))
            {
                foreach (var instruction in YieldInstance(item, provider))
                {
                    yield return instruction;
                }
            }
        }

        private IEnumerable<IInstruction> YieldInstance(object instance, IElementProvider provider) => 
            YieldInstance(instance, provider, SelectBody(instance));

        private IEnumerable<IInstruction> YieldInstance<T>(T instance, IElementProvider provider, IEnumerable<IInstruction> body)
        {
            yield return new StartContextInstruction(_contexts, context => context.New(instance));
            yield return new StartInstanceInstruction(provider);
            foreach (var instruction in body)
            {
                yield return instruction;
            }
            yield return EndCurrentElementInstruction.Default;
            yield return _endContext;
        }

        class StartContextInstruction : InstructionBase<IWritingContext>
        {
            private readonly Stack<IDisposable> _stack;
            private readonly Func<IWritingContext, IDisposable> _source;

            public StartContextInstruction(Stack<IDisposable> stack, Func<IWritingContext, IDisposable> source)
            {
                _stack = stack;
                _source = source;
            }

            protected override void OnExecute(IWritingContext services) => _stack.Push(_source(services));
        }

        class EndContextInstruction : IInstruction
        {
            private readonly Stack<IDisposable> _stack;
            public EndContextInstruction(Stack<IDisposable> stack)
            {
                _stack = stack;
            }

            public void Execute(IServiceProvider services) => _stack.Pop().Dispose();
        }

        IEnumerable<IInstruction> Dictionary(IEnumerable dictionary)
        {
            foreach (var instruction in YieldMembers(dictionary, _membersType))
            {
                yield return instruction;
            }

            foreach (DictionaryEntry item in dictionary)
            {
                var body = YieldInstance(item.Key, new ApplicationElementProvider((ns, o) => new DictionaryKeyElement(ns)))
                    .Concat(
                        YieldInstance(item.Value, new ApplicationElementProvider((ns, o) => new DictionaryValueElement(ns)))
                    );
                foreach (var instruction in YieldInstance(item, new ApplicationElementProvider((ns, o) => new DictionaryItemElement(ns)), body))
                {
                    yield return instruction;
                }
            }
        }

        readonly private static EmitCurrentInstanceInstruction[] Primitive = {EmitCurrentInstanceInstruction.Default};

        private IEnumerable<IInstruction> YieldMembers(object instance) => YieldMembers(instance, _emitType);

        private IEnumerable<IInstruction> YieldMembers(object instance, IInstruction type)
        {
            var contexts = MemberContexts.Default.Get(instance);
            var properties = contexts.Where(_specification).ToArray();

            yield return type;

            foreach (var member in properties)
            {
                yield return new StartContextInstruction(_contexts, context => context.New(member.Metadata));
                yield return new StartContextInstruction(_contexts, context => context.ToMemberContext());
                yield return EmitMemberAsTextInstruction.Default;
                yield return _endContext;
                yield return _endContext;
            }

            foreach (var member in contexts.Except(properties))
            {
                yield return new StartContextInstruction(_contexts, context => context.New(member.Metadata));
                yield return new StartInstanceInstruction(new MemberElementProvider(member));
                yield return new StartContextInstruction(_contexts, context => context.ToMemberContext());
                yield return new StartContextInstruction(_contexts, context => context.New(context.Current.Member?.Value));

                foreach (var instruction in SelectBody(member.Value))
                {
                    yield return instruction;
                }

                yield return _endContext;
                yield return _endContext;
                yield return EndCurrentElementInstruction.Default;
                yield return _endContext;
            }
        }
    }

    class StartInstanceInstruction : WriteInstructionBase
    {
        private readonly IElementProvider _provider;
        public StartInstanceInstruction(IElementProvider provider)
        {
            _provider = provider;
        }

        protected override void OnExecute(IWriting services)
        {
            var element = _provider.Get(services, services.Current.Instance);
            services.Begin(element);
        }
    }

    class StartRootInstruction : InstructionBase<IWriting>
    {
        public static StartRootInstruction Default { get; } = new StartRootInstruction();
        StartRootInstruction() {}

        protected override void OnExecute(IWriting services)
        {
            var root = services.Current.Root;
            var element = new RootElement(services.Get(root), root);
            services.Start(element);
        }
    }

    class EndCurrentElementInstruction : InstructionBase<IWriting>
    {
        public static EndCurrentElementInstruction Default { get; } = new EndCurrentElementInstruction();
        EndCurrentElementInstruction() {}

        protected override void OnExecute(IWriting services) => services.EndElement();
    }
}