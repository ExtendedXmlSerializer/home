using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWriteInstruction<in T>
    {
        void Execute(IWriting services, T instance);
    }

    public abstract class WriteInstructionBase : IInstruction
    {
        public virtual void Execute(IServiceProvider services) => Execute(services.AsValid<IWriting>());

        protected abstract void Execute(IWriting writing);

    }

    public abstract class WriteInstructionBase<T> : WriteInstructionBase, IWriteInstruction<T>
    {
        protected override void Execute(IWriting writing)
        {
            var instance = writing.Current.Instance;
            if (instance is T)
            {
                Execute(writing, (T) instance);
                return;
            }
            throw new InvalidOperationException(
                      $"Expected an instance of type '{typeof(T)}' but got an instance of '{instance.GetType()}'");
        }

        protected abstract void Execute(IWriting writing, T instance);

        void IWriteInstruction<T>.Execute( IWriting writing, T instance ) => Execute(writing, instance);
    }

    class EmitContentInstruction : WriteInstructionBase
    {
        public static EmitContentInstruction Default { get; } = new EmitContentInstruction();
        EmitContentInstruction() {}

        protected override void Execute(IWriting writing) => writing.Emit(writing.Current.Content);
    }

    class EmitDictionaryPairInstruction : WriteInstructionBase<DictionaryEntry>
    {
        private readonly IInstruction _key;
        private readonly IInstruction _value;

        public EmitDictionaryPairInstruction(IInstruction key, IInstruction value)
        {
            _key = key;
            _value = value;
        }

        protected override void Execute(IWriting writing, DictionaryEntry instance)
        {
            using (writing.New(instance.Key))
            {
                _key.Execute(writing);
            }

            using (writing.New(instance.Value))
            {
                _value.Execute(writing);
            }
        }
    }

    class EmitDictionaryInstruction : WriteInstructionBase<IDictionary>
    {
        private readonly IInstruction _entry;

        public EmitDictionaryInstruction(IInstruction key, IInstruction value) : this(
            new EmitObjectInstruction(ExtendedXmlSerializer.Item,
                                           new EmitDictionaryPairInstruction(key, value))
        ) {}

        public EmitDictionaryInstruction(IInstruction entry)
        {
            _entry = entry;
        }

        protected override void Execute(IWriting writing, IDictionary instance)
        {
            foreach (DictionaryEntry item in instance)
            {
                using (writing.New(item))
                {
                    _entry.Execute(writing);
                }
            }
        }
    }

    class EmitEnumerableInstruction : WriteInstructionBase<IEnumerable>
    {
        private readonly IInstruction _instruction;

        public EmitEnumerableInstruction(IInstruction instruction)
        {
            _instruction = instruction;
        }

        protected override void Execute(IWriting writing, IEnumerable instance)
        {
            var items = instance as Array ?? instance.Cast<object>().ToArray();
            foreach (var item in items)
            {
                using (writing.New(item))
                {
                    _instruction.Execute(writing);
                }
            }
        }
    }

    class EmitCurrentInstanceTypeInstruction : EmitTypeInstruction
    {
        public static EmitCurrentInstanceTypeInstruction Default { get; } = new EmitCurrentInstanceTypeInstruction();
        EmitCurrentInstanceTypeInstruction() : base(context => context.Current.Instance.GetType()) {}
    }

    class EmitTypeInstruction : WriteInstructionBase
    {
        private readonly Func<IWritingContext, Type> _provider;

        // public EmitTypeInstruction(Type type) : this(type.Accept) {}

        public EmitTypeInstruction(Func<IWritingContext, Type> provider)
        {
            _provider = provider;
        }

        protected override void Execute(IWriting writing) => writing.Property(ExtendedXmlSerializer.Type, _provider(writing).FullName);
    }

    class EmitContentAsMemberPropertyInstruction : WriteInstructionBase
    {
        /*private readonly MemberInfo _member;

        public EmitMemberContentInstruction(MemberInfo member)
        {
            _member = member;
        }*/

        public static EmitContentAsMemberPropertyInstruction Default { get; } = new EmitContentAsMemberPropertyInstruction();
        EmitContentAsMemberPropertyInstruction() {}

        protected override void Execute(IWriting writing) => writing.Property(writing.Current.Member.Name, writing.Current.Content);
    }

    /*class EmitMemberAsPropertyInstruction : EmitMemberInstructionBase
    {
        public EmitMemberAsPropertyInstruction(MemberInfo member) 
            : base(member, ) {}
    }*/

    /*class EmitMemberAsContentInstruction : EmitMemberInstructionBase
    {
        public EmitMemberAsContentInstruction(MemberInfo member, IInstruction content) : 
            base(member, ) {}
    }*/

    /*abstract class EmitMemberInstructionBase : DecoratedWriteInstruction
    {
        private readonly ObjectAccessors.PropertyGetter _selector;
        private readonly object _defaultValue;

        protected EmitMemberInstructionBase(MemberInfo member, IInstruction content)
            : this(new StartNewMemberContextInstruction(member, content), DefaultValues.Default.Get(member.GetMemberType()))
        {}

        protected EmitMemberInstructionBase(IInstruction content, object defaultValue)
            : base(content)
        {
            _defaultValue = defaultValue;
        }

        /*protected override void Execute(IWriting services)
        {
            if (services.Current.MemberValue != _defaultValue) // TODO: Move this to a specifica
            {
                base.Execute(services);
            }
        }#1#
    }*/

    public interface INameProvider
    {
        string Get(IServiceProvider services);
    }

    /*class InstanceTypeNameProvider : INameProvider
    {
        public static InstanceTypeNameProvider Default { get; } = new InstanceTypeNameProvider();
        InstanceTypeNameProvider() {}

        public string Get(IServiceProvider services) => services.AsValid<IWritingContext>().Current.Instance.GetType().Name;
    }*/

    class TypeDefinitionNameProvider : FixedNameProvider
    {
        public TypeDefinitionNameProvider(Type type) : base(TypeDefinitionCache.GetDefinition(type).Name) {}
    }

    class FixedNameProvider : INameProvider
    {
        private readonly string _name;

        public FixedNameProvider(string name)
        {
            _name = name;
        }

        public string Get(IServiceProvider services) => _name;
    }

    class EmitTypedContextInstruction : CompositeInstruction
    {
        public EmitTypedContextInstruction(IInstruction body) : base(EmitContextTypeInstruction.Default, body) {}
    }
    
    class EmitObjectInstruction : DecoratedWriteInstruction
    {
        private readonly INameProvider _provider;

        public EmitObjectInstruction(string name, IInstruction instruction)
            : this(new FixedNameProvider(name), instruction) {}

        public EmitObjectInstruction(MemberInfo member, IInstruction instruction)
            : this(new MemberInfoNameProvider(member), instruction) {}

        public EmitObjectInstruction(Type type, IInstruction instruction)
            : this(new TypeDefinitionNameProvider(type), instruction) {}

        public EmitObjectInstruction(INameProvider provider, IInstruction instruction) : base(instruction)
        {
            _provider = provider;
        }

        protected override void Execute(IWriting writing)
        {
            var name = _provider.Get(writing);
            writing.StartObject(name);
            base.Execute(writing);
            writing.EndObject();
        }
    }

    class DecoratedWriteInstruction : WriteInstructionBase
    {
        private readonly IInstruction _instruction;
        public DecoratedWriteInstruction(IInstruction instruction)
        {
            _instruction = instruction;
        }

        protected override void Execute(IWriting writing) => _instruction.Execute(writing);
    }

    abstract class NewWriteContextInstructionBase : DecoratedWriteInstruction
    {
        protected NewWriteContextInstructionBase(IInstruction instruction) : base(instruction) {}

        protected override void Execute(IWriting writing)
        {
            using (DetermineContext(writing))
            {
                if (writing.Starting(writing))
                {
                    base.Execute(writing);
                }
                writing.Finished(writing);
            }
        }

        protected abstract IDisposable DetermineContext(IWriting writing);
    }

    class StartNewMembersContextInstruction : NewWriteContextInstructionBase
    {
        private readonly IImmutableList<MemberInfo> _members;
        public StartNewMembersContextInstruction(IImmutableList<MemberInfo> members, IInstruction instruction) : base(instruction)
        {
            _members = members;
        }
        protected override IDisposable DetermineContext(IWriting writing) => writing.New(_members);
    }

    class StartNewValueContextInstruction : StartNewValueContextInstructionBase
    {
        public static StartNewValueContextInstruction Default { get; } = new StartNewValueContextInstruction();
        StartNewValueContextInstruction() : this(writing => writing.Current.Instance) {}

        private readonly Func<IWriting, object> _source;
        public StartNewValueContextInstruction(Func<IWriting, object> source) : base(EmitContentInstruction.Default)
        {
            _source = source;
        }

        protected override object DetermineInstance(IWriting writing) => _source(writing);
    }

    abstract class StartNewValueContextInstructionBase : NewWriteContextInstructionBase
    {
        protected StartNewValueContextInstructionBase(IInstruction instruction) : base(instruction) {}

        protected abstract object DetermineInstance(IWriting writing);

        protected override IDisposable DetermineContext(IWriting writing)
        {
            var instance = DetermineInstance(writing);
            var value = writing.Serialize(instance);
            var result = writing.New(value);
            return result;
        }
    }

    class StartNewMemberValueAsValueContextInstruction : StartNewValueContextInstructionBase
    {
        public static StartNewMemberValueAsValueContextInstruction Default { get; } = new StartNewMemberValueAsValueContextInstruction();
        StartNewMemberValueAsValueContextInstruction() : base(EmitContentAsMemberPropertyInstruction.Default) {}
        
        protected override object DetermineInstance(IWriting writing)
        {
            switch (writing.Current.State)
            {
                case WriteState.MemberValue:
                    var result = writing.Current.MemberValue;
                    return result;
            }
            throw new InvalidOperationException($"A call was made to serialize the member value of '{writing.Current.Member}', but it has not been set.");
        }
    }

    /*class StartNewMemberValueContextInstruction : NewWriteContextInstructionBase
    {
        public StartNewMemberValueContextInstruction(IInstruction instruction) : base(instruction) {}
        protected override IDisposable DetermineContext(IWriting context)
        {
            var getter = Getters.Default.Get(context.Current.Member);
            var value = getter.Invoke(context.Current.Instance);
            var memberValue = new MemberValue(value);
            var result = context.New(memberValue);
            return result;
        }
    }*/

    class EmitMemberAsPropertyInstruction : DecoratedWriteInstruction, IPropertyInstruction
    {
        readonly private static StartNewMemberValueContextInstruction Emit =
            new StartNewMemberValueContextInstruction(StartNewMemberValueAsValueContextInstruction.Default);

        public EmitMemberAsPropertyInstruction(MemberInfo member)
            : this(new StartNewMemberContextInstruction(member, Emit)) {}

        public EmitMemberAsPropertyInstruction(IInstruction instruction) : base(instruction) {}
    }

    class EmitMemberAsContentInstruction : DecoratedWriteInstruction, IContentInstruction
    {
        public EmitMemberAsContentInstruction(MemberInfo member, IInstruction instruction) : this(
            new StartNewMemberContextInstruction(member, 
                new EmitObjectInstruction(member, new StartNewMemberValueContextInstruction(instruction))
            )
        ) {}

        public EmitMemberAsContentInstruction(IInstruction instruction) : base(instruction) {}
    }

    class StartNewMemberValueContextInstruction : NewWriteContextInstructionBase
    {
        public StartNewMemberValueContextInstruction(IInstruction content) : base(content) {}
        protected override IDisposable DetermineContext(IWriting writing) => writing.NewMemberValue();
    }

    class StartNewMemberContextInstruction : NewWriteContextInstructionBase
    {
        private readonly MemberInfo _member;

        public StartNewMemberContextInstruction(MemberInfo member, IInstruction instruction) : base(instruction)
        {
            _member = member;
        }

        protected override IDisposable DetermineContext(IWriting writing) => writing.New(_member);
    }

    class StartNewContextFromRootInstruction : NewWriteContextInstructionBase
    {
        public StartNewContextFromRootInstruction(IInstruction instruction) : base(instruction) {}

        protected override IDisposable DetermineContext(IWriting writing) => 
            writing.New(writing.Current.Root);
    }
}