using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWriteInstruction<in T>
    {
        void Execute(IWritingServices services, T instance);
    }

    public abstract class WriteInstructionBase : IInstruction
    {
        public virtual void Execute(IServiceProvider services) => Execute(services.AsValid<IWritingServices>());

        protected abstract void Execute(IWritingServices services);

    }

    public abstract class WriteInstructionBase<T> : WriteInstructionBase, IWriteInstruction<T>
    {
        protected override void Execute(IWritingServices services)
        {
            var instance = services.Current.Instance;
            if (instance is T)
            {
                Execute(services, (T) instance);
                return;
            }
            throw new InvalidOperationException(
                      $"Expected an instance of type '{typeof(T)}' but got an instance of '{instance.GetType()}'");
        }

        protected abstract void Execute(IWritingServices services, T instance);

        void IWriteInstruction<T>.Execute( IWritingServices services, T instance ) => Execute(services, instance);
    }

    class EmitContentInstruction : WriteInstructionBase
    {
        public static EmitContentInstruction Default { get; } = new EmitContentInstruction();
        EmitContentInstruction() {}

        protected override void Execute(IWritingServices services) => services.Emit(services.Current.Content);
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

        protected override void Execute(IWritingServices services, DictionaryEntry instance)
        {
            using (services.New(instance.Key))
            {
                _key.Execute(services);
            }

            using (services.New(instance.Value))
            {
                _value.Execute(services);
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

        protected override void Execute(IWritingServices services, IDictionary instance)
        {
            foreach (DictionaryEntry item in instance)
            {
                using (services.New(item))
                {
                    _entry.Execute(services);
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

        protected override void Execute(IWritingServices services, IEnumerable instance)
        {
            var items = instance as Array ?? instance.Cast<object>().ToArray();
            foreach (var item in items)
            {
                using (services.New(item))
                {
                    _instruction.Execute(services);
                }
            }
        }
    }

    class EmitTypeInstruction : WriteInstructionBase
    {
        readonly Type _type;

        public EmitTypeInstruction(Type type)
        {
            _type = type;
        }

        protected override void Execute(IWritingServices services)
        {
            if (services.Current.Instance.GetType() != _type)
            {
                services.Property(ExtendedXmlSerializer.Type, _type.FullName);
            }
        }
    }

    class EmitMemberContentInstruction : WriteInstructionBase
    {
        private readonly MemberInfo _member;

        public EmitMemberContentInstruction(MemberInfo member)
        {
            _member = member;
        }

        protected override void Execute(IWritingServices services) => services.Property(_member.Name, services.Current.Content);
    }

    class EmitMemberAsPropertyInstruction : EmitMemberInstructionBase
    {
        public EmitMemberAsPropertyInstruction(MemberInfo member) : base(member, new StartNewContentContextInstruction(new EmitMemberContentInstruction(member))) {}
    }

    class EmitMemberAsContentInstruction : EmitMemberInstructionBase
    {
        public EmitMemberAsContentInstruction(MemberInfo member, IInstruction content) : base(member, content) {}
    }

    abstract class EmitMemberInstructionBase : DecoratedWriteInstruction
    {
        private readonly ObjectAccessors.PropertyGetter _selector;
        private readonly object _defaultValue;

        protected EmitMemberInstructionBase(MemberInfo member, IInstruction content)
            : this(member, Getters.Default.Get(member), content, DefaultValues.Default.Get(member.GetMemberType()))
        {}

        protected EmitMemberInstructionBase(MemberInfo member, ObjectAccessors.PropertyGetter selector, IInstruction content, object defaultValue)
            : base(new EmitObjectInstruction(member, content))
        {
            _selector = selector;
            _defaultValue = defaultValue;
        }

        protected override void Execute(IWritingServices services)
        {
            var value = _selector(services.Current.Instance);
            if (value != _defaultValue)
            {
                using (services.New(value))
                {
                    base.Execute(services);
                }
            }
        }
    }

    public interface INameProvider
    {
        string Get(IServiceProvider services);
    }

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

        protected override void Execute(IWritingServices services)
        {
            services.StartObject(_provider.Get(services));
            base.Execute(services);
            services.EndObject();
        }
    }

    class DecoratedWriteInstruction : WriteInstructionBase
    {
        private readonly IInstruction _instruction;
        public DecoratedWriteInstruction(IInstruction instruction)
        {
            _instruction = instruction;
        }

        protected override void Execute(IWritingServices services) => _instruction.Execute(services);
    }

    abstract class NewWriteContextInstructionBase : DecoratedWriteInstruction
    {
        protected NewWriteContextInstructionBase(IInstruction instruction) : base(instruction) {}

        protected override void Execute(IWritingServices services)
        {
            using (DetermineContext(services))
            {
                if (services.Starting(services))
                {
                    base.Execute(services);
                }
                services.Finished(services);
            }
        }

        protected abstract IDisposable DetermineContext(IWritingServices context);
    }

    class StartNewMembersContextInstruction : NewWriteContextInstructionBase
    {
        private readonly IImmutableList<MemberInfo> _members;
        public StartNewMembersContextInstruction(IImmutableList<MemberInfo> members, IInstruction instruction) : base(instruction)
        {
            _members = members;
        }
        protected override IDisposable DetermineContext(IWritingServices context) => context.New(_members);
    }

    class StartNewContentContextInstruction : NewWriteContextInstructionBase
    {
        public static StartNewContentContextInstruction Default { get; } = new StartNewContentContextInstruction();
        StartNewContentContextInstruction() : base(EmitContentInstruction.Default) {}

        public StartNewContentContextInstruction(IInstruction instruction) : base(instruction) {}

        protected override IDisposable DetermineContext(IWritingServices context) =>
            context.New(context.Serialize(context.Current.Instance));
    }

    class StartNewMemberContextInstruction : NewWriteContextInstructionBase
    {
        private readonly MemberInfo _member;

        public StartNewMemberContextInstruction(MemberInfo member, IInstruction instruction) : base(instruction)
        {
            _member = member;
        }

        protected override IDisposable DetermineContext(IWritingServices context) => context.New(_member);
    }

    class StartNewContextFromRootInstruction : NewWriteContextInstructionBase
    {
        public StartNewContextFromRootInstruction(IInstruction instruction) : base(instruction) {}

        protected override IDisposable DetermineContext(IWritingServices context) => 
            context.New(context.Current.Root);
    }
}