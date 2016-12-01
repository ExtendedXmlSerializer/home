using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWritePlanComposer
    {
        IWritePlan Compose();
    }

    class MemberInfoNameProvider : FixedNameProvider
    {
        public MemberInfoNameProvider(MemberInfo member) 
            : base(member.GetCustomAttribute<XmlAttributeAttribute>()?.AttributeName ?? member.Name) {}
    }

    public interface IInstructionSpecification : ISpecification<object>
    {
        bool Defer(MemberInfo member);
    }

    abstract class InstructionSpecificationBase : IInstructionSpecification
    {
        public abstract bool IsSatisfiedBy(object parameter);
        
        public virtual bool Defer(MemberInfo member) => false;
    }

    class SpecificationCandidatesSelector : IParameterizedSource<object, IEnumerable<object>>
    {
        public static SpecificationCandidatesSelector Default { get; } = new SpecificationCandidatesSelector();
        SpecificationCandidatesSelector() {}

        public IEnumerable<object> Get(object parameter)
        {
            yield return parameter;
            var member = parameter as MemberInfo;
            if (member != null)
            {
                yield return member.GetMemberType();
            }

            if (parameter is WriteContext)
            {
                var context = (WriteContext)parameter;
                yield return context.Member;
                yield return context.Member.GetMemberType();
            }

        }
    }

    class InstructionSpecification : InstructionSpecificationBase
    {
        private readonly IParameterizedSource<object, IEnumerable<object>> _candidates;
        private readonly Func<MemberInfo, bool> _defer;
        private readonly Func<object, bool>[] _specifications;
        public static InstructionSpecification Default { get; } = new InstructionSpecification();
        InstructionSpecification()
            : this(
                SpecificationCandidatesSelector.Default, context => false,
                IsTypeSpecification<IAttachedProperty>.Default.Adapt().IsSatisfiedBy) {}

        public InstructionSpecification(IParameterizedSource<object, IEnumerable<object>> candidates,
                                        Func<MemberInfo, bool> defer, params Func<object, bool>[] specifications)
        {
            _candidates = candidates;
            _defer = defer;
            _specifications = specifications;
        }

        public override bool IsSatisfiedBy(object parameter)
        {
            var candidates = _candidates.Get(parameter);
            foreach (var candidate in candidates)
            {
                foreach (var specification in _specifications)
                {
                    if (specification(candidate))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool Defer(MemberInfo member) => _defer(member);
    }

    class IsPropertyMemberSpecification : ISpecification<MemberInfo>
    {
        public static IsPropertyMemberSpecification Default { get; } = new IsPropertyMemberSpecification();
        IsPropertyMemberSpecification() {}

        public bool IsSatisfiedBy(MemberInfo member) => member.IsDefined(typeof(XmlAttributeAttribute));
    }

    class IsPrimitiveSpecification : ISpecification<Type>
    {
        public static IsPrimitiveSpecification Default { get; } = new IsPrimitiveSpecification();
        IsPrimitiveSpecification() {}

        public bool IsSatisfiedBy(Type parameter) => TypeDefinitionCache.GetDefinition(parameter).IsPrimitive;
    }

    class AutoAttributeSpecification : InstructionSpecification
    {
        public new static AutoAttributeSpecification Default { get; } = new AutoAttributeSpecification();
        AutoAttributeSpecification() : base(
            SpecificationCandidatesSelector.Default,
            info => info.GetMemberType() == typeof(string),
            IsPrimitiveSpecification.Default.Adapt().IsSatisfiedBy,
            IsPropertyMemberSpecification.Default.Adapt().IsSatisfiedBy,
            AutoAttributeValueSpecification.Default.Adapt().IsSatisfiedBy
        ) {}
    }

    class AutoAttributeWritePlanComposer : DefaultWritePlanComposer
    {
        public new static AutoAttributeWritePlanComposer Default { get; } = new AutoAttributeWritePlanComposer();
        AutoAttributeWritePlanComposer() : base(AutoAttributeSpecification.Default) {}
    }

    class AutoAttributeValueSpecification : ISpecification<WriteContext>
    {
        readonly private static Type Type = typeof(string);

        public static AutoAttributeValueSpecification Default { get; } = new AutoAttributeValueSpecification();
        AutoAttributeValueSpecification() : this(128) {}

        private readonly int _maxLength;

        public AutoAttributeValueSpecification(int maxLength)
        {
            _maxLength = maxLength;
        }

        public bool IsSatisfiedBy(WriteContext parameter)
        {
            if (parameter.Member.GetMemberType() == Type)
            {
                var value = Getters.Default.Get(parameter.Member).Invoke(parameter.Instance) as string;
                var result = value?.Length < _maxLength;
                return result;
            }
            return false;
        }
    }

    class AdditionalInstructions : IParameterizedSource<IWritePlan, IImmutableList<IWritePlan>>
    {
        public static AdditionalInstructions Default { get; } = new AdditionalInstructions();
        AdditionalInstructions() {}

        public IImmutableList<IWritePlan> Get(IWritePlan parameter) => ImmutableList.Create<IWritePlan>();
    }

    class DefaultWritePlanComposer : IWritePlanComposer
    {
        public static DefaultWritePlanComposer Default { get; } = new DefaultWritePlanComposer();
        DefaultWritePlanComposer() : this(InstructionSpecification.Default) {}

        private readonly IInstructionSpecification _specification;
        private readonly IParameterizedSource<IWritePlan, IImmutableList<IWritePlan>> _additional;

        public DefaultWritePlanComposer(IInstructionSpecification specification) : this(specification, AdditionalInstructions.Default) {}

        public DefaultWritePlanComposer(IInstructionSpecification specification,
                                          IParameterizedSource<IWritePlan, IImmutableList<IWritePlan>> additional)
        {
            _specification = specification;
            _additional = additional;
        }

        public IWritePlan Compose()
        {
            var collection = new List<IWritePlan>();
            var selector = new CachedWritePlan(new FirstAssignedWritePlan(collection));
            collection.AddRange(
                new IWritePlan[]
                    {
                        PrimitiveWritePlan.Default,
                        new DictionaryWritePlan(selector),
                        new EnumerableWritePlan(selector),
                        new ObjectWritePlan(selector, _specification),
                        new GeneralObjectWritePlan(selector), 
                    }
                    .Concat(_additional.Get(selector))
            );
            var result = new CachedWritePlan(new RootWritePlan(selector));
            return result;
        }
    }

    public interface IWritePlan
    {
        IInstruction For(Type type);
    }

    class ConditionalWritePlan : ConditionalWritePlanBase
    {
        private readonly IWritePlan _builder;
        public ConditionalWritePlan(Func<Type, bool> specification, IWritePlan builder) : base(specification)
        {
            _builder = builder;
        }
        protected override IInstruction PerformBuild(Type type) => _builder.For(type);
    }

    abstract class ConditionalWritePlanBase : IWritePlan
    {
        private readonly Func<Type, bool> _specification;

        protected ConditionalWritePlanBase(Func<Type, bool> specification)
        {
            _specification = specification;
        }

        public IInstruction For(Type type) => _specification(type) ? PerformBuild(type) : null;
        protected abstract IInstruction PerformBuild(Type type);
    }

    class SerializableMembers : WeakCacheBase<Type, IImmutableList<MemberInfo>>
    {
        public static SerializableMembers Default { get; } = new SerializableMembers();
        SerializableMembers() : this(_ => true) {}

        private readonly Func<TypeDefinition, bool> _serializable;

        public SerializableMembers(Func<TypeDefinition, bool> serializable)
        {
            _serializable = serializable;
        }

        protected override IImmutableList<MemberInfo> Callback(Type key) => GetWritableMembers(key).ToImmutableList();

        IEnumerable<MemberInfo> GetWritableMembers(Type type)
        {
            foreach (var member in TypeDefinitionCache.GetDefinition(type).Properties)
            {
                if (_serializable(member.TypeDefinition))
                {
                    yield return member.MemberInfo;
                }
            }
        }
    }

    public interface IMemberInstructionFactory
    {
        IInstruction Property(MemberInfo member);
        IInstruction Content(MemberInfo member);
    }

    class ObjectMemberWritePlan : IWritePlan
    {
        readonly private static Func<Type, IImmutableList<MemberInfo>> Members = SerializableMembers.Default.Get;

        private readonly IWritePlan _primary;
        private readonly Func<object, bool> _specification;
        private readonly Func<MemberInfo, bool> _deferred;
        private readonly Func<Type, IImmutableList<MemberInfo>> _members;
        private readonly Func<MemberInfo, IInstruction> _property, _content;

        public ObjectMemberWritePlan(IWritePlan primary, IInstructionSpecification specification)
            : this(primary, specification, specification.Defer, new MemberInstructionFactory(primary)) {}

        public ObjectMemberWritePlan(IWritePlan primary, IInstructionSpecification specification, Func<MemberInfo, bool> deferred, IMemberInstructionFactory factory)
            : this(primary, specification.IsSatisfiedBy, deferred, factory.Property, factory.Content, Members) {}

        public ObjectMemberWritePlan(
            IWritePlan primary,
            Func<object, bool> specification, 
            Func<MemberInfo, bool> deferred, 
            Func<MemberInfo, IInstruction> property,
            Func<MemberInfo, IInstruction> content, 
            Func<Type, IImmutableList<MemberInfo>> members
        ) {
            _primary = primary;
            _specification = specification;
            _deferred = deferred;
            _property = property;
            _content = content;
            _members = members;
        }

        public IInstruction For(Type type)
        {
            var all = _members(type);
            var deferred = all.Where(_deferred).ToImmutableList();
            var members = all.Except(deferred).ToArray();
            var properties = members.Where<MemberInfo>(_specification).ToArray();
            var contents = members.Except(properties);
            
            var body = new CompositeInstruction(
                new CompositeInstruction(properties.Select(_property).ToImmutableList()),
                new EmitDeferredMembersInstruction(deferred, _property, _content, _specification, new EmitAttachedPropertiesInstruction(_primary, _specification)),
                new CompositeInstruction(contents.Select(_content).ToImmutableList())
            );
            var result = new StartNewMembersContextInstruction(all, body);
            return result;
        }
    }

    class EmitAttachedPropertiesInstruction : WriteInstructionBase
    {
        private readonly IWritePlan _primary;
        private readonly Func<object, bool> _specification;
        public EmitAttachedPropertiesInstruction(IWritePlan primary, Func<object, bool> specification)
        {
            _primary = primary;
            _specification = specification;
        }

        protected override void Execute(IWriting services)
        {
            var all = services.GetProperties();
            var properties = Properties(all).ToArray();
            foreach (var property in properties)
            {
                services.Property(property);
            }

            foreach (var content in all.Except(properties))
            {
                new EmitObjectInstruction(content.Name, _primary.For(content.Value.GetType())).Execute(services);
            }
        }

        IEnumerable<IAttachedProperty> Properties(IEnumerable<IAttachedProperty> source)
        {
            foreach (var property in source)
            {
                if (_specification(property))
                {
                    yield return property;
                }
            }
        }
    }

    class EmitDeferredMembersInstruction : DecoratedWriteInstruction
    {
        private readonly IImmutableList<MemberInfo> _members;
        private readonly Func<MemberInfo, IInstruction> _property, _content;
        private readonly Func<object, bool> _specification;

        public EmitDeferredMembersInstruction(IImmutableList<MemberInfo> members, Func<MemberInfo, IInstruction> property,
                                              Func<MemberInfo, IInstruction> content,
                                              Func<object, bool> specification, IInstruction instruction) : base(instruction)
        {
            _members = members;
            _property = property;
            _content = content;
            _specification = specification;
        }

        protected override void Execute(IWriting services)
        {
            var properties = Properties(services.Current).ToArray();
            
            foreach (var instruction in properties.Select(_property))
            {
                instruction.Execute(services);
            }

            base.Execute(services);

            foreach (var instruction in _members.Except(properties).Select(_content))
            {
                instruction.Execute(services);
            }
        }

        IEnumerable<MemberInfo> Properties(WriteContext current)
        {
            foreach (var member in _members)
            {
                if (_specification(new WriteContext(current.Root, current.Instance, _members, member, null)))
                {
                    yield return member;
                }
            }
        }
    }

    class MemberInstructionFactory : IMemberInstructionFactory
    {
        readonly private static Func<MemberInfo, IInstruction> PropertyDelegate = CreateProperty;
        
        private readonly IWritePlan _writePlan;
        readonly private Func<MemberInfo, IInstruction> _property, _content;

        public MemberInstructionFactory(IWritePlan primary)
        {
            _writePlan = primary;
            _property = new Wrap(PropertyDelegate).Get;
            _content = new Wrap(CreateContent).Get;
        }

        public IInstruction Property(MemberInfo member) => _property(member);
        static IInstruction CreateProperty(MemberInfo member) => new EmitMemberAsPropertyInstruction(member);

        public IInstruction Content(MemberInfo member) => _content(member);
        IInstruction CreateContent(MemberInfo member) =>
            new EmitMemberAsContentInstruction(
                    member,
                    new DeferredInstruction(
                            new FixedDecoratedWritePlan(
                                _writePlan, member.GetMemberType()
                            ).Get
                        )
                );
        
        sealed class Wrap
        {
            private readonly Func<MemberInfo, IInstruction> _factory;
            public Wrap(Func<MemberInfo, IInstruction> factory)
            {
                _factory = factory;
            }

            public IInstruction Get(MemberInfo member) => new StartNewMemberContextInstruction(member, _factory(member));
        }
    }

    class GeneralObjectWritePlan : ConditionalWritePlan
    {
        public GeneralObjectWritePlan(IWritePlan plan) : base(type => type == typeof(object), new FixedWritePlan(new EmitGeneralObjectInstruction(plan))) {}
    }

    class EmitGeneralObjectInstruction : WriteInstructionBase
    {
        private readonly IWritePlan _selector;
        public EmitGeneralObjectInstruction(IWritePlan selector)
        {
            _selector = selector;
        }

        protected override void Execute(IWriting services)
        {
            var type = services.Current.Instance.GetType();
            var selected = _selector.For(type);
            selected?.Execute(services);
        }
    }

    class ObjectWritePlan : ConditionalWritePlanBase
    {
        private readonly IWritePlan _members;

        public ObjectWritePlan(IWritePlan primary, IInstructionSpecification specification) 
            : this(new ObjectMemberWritePlan(primary, specification)) {}

        public ObjectWritePlan(IWritePlan members) : base(type => TypeDefinitionCache.GetDefinition(type).IsObjectToSerialize)
        {
            _members = members;
        }

        protected override IInstruction PerformBuild(Type type) => 
            new CompositeInstruction(EmitCurrentInstanceTypeInstruction.Default, _members.For(type));
    }

    class RootWritePlan : IWritePlan
    {
        private readonly IWritePlan _builder;

        public RootWritePlan(IWritePlan builder)
        {
            _builder = builder;
        }

        public IInstruction For(Type type)
        {
            var content = _builder.For(type);
            if (content == null)
            {
                throw new InvalidOperationException($"Could not find instruction for type '{type}'");
            }
            var result = new StartNewContextFromRootInstruction(new EmitObjectInstruction(type, content));
            return result;
        }
    }

    class CachedWritePlan : IWritePlan
    {
        private readonly ConditionalWeakTable<Type, IInstruction> _cache =
            new ConditionalWeakTable<Type, IInstruction>();

        private readonly ConditionalWeakTable<Type, IInstruction>.CreateValueCallback _callback;

        public CachedWritePlan(IWritePlan inner) : this(inner.For) {}

        public CachedWritePlan(ConditionalWeakTable<Type, IInstruction>.CreateValueCallback callback)
        {
            _callback = callback;
        }

        public IInstruction For(Type type) => _cache.GetValue(type, _callback);
    }

    class EnumerableWritePlan : ConditionalWritePlanBase
    {
        readonly private static Func<Type, bool> Specification = IsSatisfiedBy;

        private readonly IWritePlan _primary;

        public EnumerableWritePlan(IWritePlan primary) : base(Specification)
        {
            _primary = primary;
        }

        private static bool IsSatisfiedBy(Type type)
        {
            var definition = TypeDefinitionCache.GetDefinition(type);
            var result = definition.IsArray || definition.IsEnumerable;
            return result;
        }

        protected override IInstruction PerformBuild(Type type)
        {
            var elementType = ElementTypeLocator.Default.Locate(type);
            var template = new EmitObjectInstruction(elementType, _primary.For(elementType));
            var result = new EmitTypedContextInstruction(new EmitEnumerableInstruction(template));
            return result;
        }
    }

    class EmitMemberTypeSpecification : ISpecification<IWritingContext>
    {
        public static EmitMemberTypeSpecification Default { get; } = new EmitMemberTypeSpecification();
        EmitMemberTypeSpecification() {}
        public bool IsSatisfiedBy(IWritingContext parameter)
        {
            var context = parameter.GetMemberContext();
            var result = context != null && context.Value.Member.IsWritable() && parameter.Current.Instance.GetType() != context?.Member.GetMemberType();
            return result;
        }
    }

    class ConditionalWriteInstruction : DecoratedWriteInstruction
    {
        private readonly ISpecification<IWritingContext> _specification;
        
        public ConditionalWriteInstruction(ISpecification<IWritingContext> specification, IInstruction instruction) : base(instruction)
        {
            _specification = specification;
        }

        protected override void Execute(IWriting services)
        {
            if (_specification.IsSatisfiedBy(services))
            {
                base.Execute(services);
            }
        }
    }

    class FirstAssignedWritePlan : IWritePlan
    {
        readonly ICollection<IWritePlan> _providers;

        public FirstAssignedWritePlan(ICollection<IWritePlan> providers)
        {
            _providers = providers;
        }

        public IInstruction For(Type type)
        {
            foreach (var provider in _providers)
            {
                var instruction = provider.For(type);
                if (instruction != null)
                {
                    return instruction;
                }
            }
            return null;
        }
    }

    class FixedWritePlan : IWritePlan
    {
        private readonly IInstruction _instruction;
        public FixedWritePlan(IInstruction instruction)
        {
            _instruction = instruction;
        }

        public IInstruction For(Type type) => _instruction;
    }

    class EmitContextTypeInstruction : ConditionalWriteInstruction
    {
        public static EmitContextTypeInstruction Default { get; } = new EmitContextTypeInstruction();
        EmitContextTypeInstruction() : base(EmitMemberTypeSpecification.Default, EmitCurrentInstanceTypeInstruction.Default) {}
    }

    class PrimitiveWritePlan : ConditionalWritePlan
    {
        readonly private static IWritePlan Emit =
            new FixedWritePlan(
                new CompositeInstruction(
                    EmitContextTypeInstruction.Default,
                    StartNewContentContextInstruction.Default
                    )
                );
            
        public static PrimitiveWritePlan Default { get; } = new PrimitiveWritePlan();
        PrimitiveWritePlan() : base(type => TypeDefinitionCache.GetDefinition(type).IsPrimitive, Emit) {}
    }

    class DictionaryWritePlan : ConditionalWritePlanBase
    {
        private readonly IWritePlan _builder;
        
        public DictionaryWritePlan(IWritePlan builder) : base(type => TypeDefinitionCache.GetDefinition(type).IsDictionary)
        {
            _builder = builder;
        }

        protected override IInstruction PerformBuild(Type type)
        {
            var arguments = TypeDefinitionCache.GetDefinition(type).GenericArguments;
            if (arguments.Length != 2)
            {
                throw new InvalidOperationException(
                          $"Attempted to write type '{type}' as a dictionary, but it does not have enough generic arguments.");
            }
            var keys = new EmitObjectInstruction(ExtendedXmlSerializer.Key, _builder.For(arguments[0]));
            var values = new EmitObjectInstruction(ExtendedXmlSerializer.Value, _builder.For(arguments[1]));
            var result = new EmitTypedContextInstruction(new EmitDictionaryInstruction(keys, values));
            return result;
        }
    }
}