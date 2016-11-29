using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization.Write
{
    public interface IWriteInstruction<in T>
    {
        void Execute(IWriteServices services, T instance);
    }

public abstract class WriteInstructionBase : WriteInstructionBase<object> {}
    public abstract class WriteInstructionBase<T> : IWriteInstruction<T>, IInstruction
    {
        public virtual void Execute(IServiceProvider services, object instance)
        {
            if (instance is T)
            {
                Execute(services.AsValid<IWriteServices>(), (T)instance);
                return;
            }
            throw new InvalidOperationException(
                      $"Expected an instance of type '{typeof(T)}' but got an instance of '{instance.GetType()}'");
        }

        protected abstract void Execute(IWriteServices services, T instance);

        void IWriteInstruction<T>.Execute( IWriteServices services, T instance ) => Execute(services, instance);
    }

    class EmitContentInstruction : WriteInstructionBase
    {
        public static EmitContentInstruction Default { get; } = new EmitContentInstruction();
        EmitContentInstruction() {}

        protected override void Execute(IWriteServices services, object instance)
        {
            var content = services.Serialize(instance);
            services.Emit(content);
        }
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

        protected override void Execute(IWriteServices services, DictionaryEntry instance)
        {
            _key.Execute(services, instance.Key);
            _value.Execute(services, instance.Value);
        }
    }

    class EmitDictionaryInstruction : WriteInstructionBase<IDictionary>
    {
        private readonly IWriteInstruction<DictionaryEntry> _entry;

        public EmitDictionaryInstruction(IInstruction key, IInstruction value) : this(
            new EmitObjectInstruction<DictionaryEntry>(ExtendedXmlSerializer.Item,
                                                       new EmitDictionaryPairInstruction(key, value))
        ) {}

        public EmitDictionaryInstruction(IWriteInstruction<DictionaryEntry> entry)
        {
            _entry = entry;
        }

        protected override void Execute(IWriteServices services, IDictionary instance)
        {
            foreach (DictionaryEntry item in instance)
            {
                _entry.Execute(services, item);
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

        protected override void Execute(IWriteServices services, IEnumerable instance)
        {
            var items = instance as Array ?? instance.Cast<object>().ToArray();
            foreach (var item in items)
            {
                _instruction.Execute(services, item);
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

        protected override void Execute(IWriteServices services, object instance)
        {
            if (instance.GetType() != _type)
            {
                services.Member(ExtendedXmlSerializer.Type, _type.FullName);
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

        protected override void Execute(IWriteServices services, object instance)
        {
            var content = services.Serialize(instance);
            services.Member(_member.Name, content);
        }
    }

    class EmitMemberInstruction<T> : EmitObjectInstruction<T>
    {
        private readonly MemberInfo _member;
        private readonly Func<object, object> _selector;
        private readonly object _defaultValue;

        public EmitMemberInstruction(PropertieDefinition definition, IInstruction content)
            : this(definition.MemberInfo, definition.GetValue, content, DefaultValues.Default.Get(definition.TypeDefinition.Type))
        {}

        public EmitMemberInstruction(MemberInfo member, Func<object, object> selector, IInstruction content, object defaultValue)
            : base(member.Name, content)
        {
            _member = member;
            _selector = selector;
            _defaultValue = defaultValue;
        }

        protected override void Execute(IWriteServices services, T instance)
        {
            var value = _selector(instance);
            if (value != _defaultValue)
            {
                base.Execute(services, value);
            }
        }
    }

    class EmitObjectInstruction : EmitObjectInstruction<object>
    {
        public EmitObjectInstruction(string name, IInstruction instruction) : base(name, instruction) {}
    }

    class EmitObjectInstruction<T> : DecoratedWriteInstruction<T>
    {
        private readonly string _name;
        
        public EmitObjectInstruction(string name, IInstruction instruction) : base(instruction)
        {
            _name = name;
        }

        protected override void Execute(IWriteServices services, T instance)
        {
            services.StartObject(_name);
            base.Execute(services, instance);
            services.EndObject();
        }
    }

    class DecoratedWriteInstruction<T> : WriteInstructionBase<T>
    {
        private readonly IInstruction _instruction;
        public DecoratedWriteInstruction(IInstruction instruction)
        {
            _instruction = instruction;
        }

        protected override void Execute(IWriteServices services, T instance) => _instruction.Execute(services, instance);
    }
}