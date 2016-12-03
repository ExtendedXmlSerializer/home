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
            : base(
                member.GetCustomAttribute<XmlAttributeAttribute>()?.AttributeName.NullIfEmpty() ??
                member.GetCustomAttribute<XmlElementAttribute>()?.ElementName.NullIfEmpty() ?? member.Name) {}
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

    class SpecificationCandidatesSelector : IFactory<object, IEnumerable<object>>
    {
        public static SpecificationCandidatesSelector Default { get; } = new SpecificationCandidatesSelector();
        SpecificationCandidatesSelector() {}

        public IEnumerable<object> Create(object parameter)
        {
            yield return parameter;
            var member = parameter as MemberInfo;
            if (member != null)
            {
                yield return member.GetMemberType();
            }

            if (parameter is MemberContext)
            {
                var context = (MemberContext)parameter;
                yield return context.Metadata;
                yield return context.MemberType;
            }

        }
    }

    public interface IInstructionCandidateSpecification : ISpecification<object>
    {
        bool Handles(object candidate);
    }

    class InstructionCandidateSpecification<T> : InstructionCandidateSpecification
    {
        public InstructionCandidateSpecification(ISpecification<T> specification) : base(o => o is T, specification.Adapt().IsSatisfiedBy) {}
    }

    class InstructionCandidateSpecification : DelegatedSpecification<object>, IInstructionCandidateSpecification
    {
        private readonly Func<object, bool> _handles;

        public InstructionCandidateSpecification(Func<object, bool> specification) : this(specification, o => true) {}

        public InstructionCandidateSpecification(Func<object, bool> handles, Func<object, bool> specification) : base(specification)
        {
            _handles = handles;
        }

        public virtual bool Handles(object candidate) => _handles(candidate);
    }

    abstract class MemberValueSpecificationBase<T> : InstructionCandidateSpecificationBase<MemberContext>
    {
        public override bool Handles(object candidate) => 
            base.Handles(candidate) && ((MemberContext)candidate).Value is T;
    }

    abstract class MemberContextSpecificationBase<T> : InstructionCandidateSpecificationBase<MemberContext>
    {
        public override bool Handles(object candidate) => 
            base.Handles(candidate) && typeof(T).IsAssignableFrom(((MemberContext)candidate).MemberType);
    }

    abstract class InstructionCandidateSpecificationBase<T> : SpecificationAdapterBase<T>, IInstructionCandidateSpecification
    {
        public virtual bool Handles(object candidate) => candidate is T;
    }

    class InstructionSpecification : InstructionSpecificationBase
    {
        private readonly IFactory<object, IEnumerable<object>> _candidates;
        private readonly Func<MemberInfo, bool> _defer;
        private readonly IImmutableList<IInstructionCandidateSpecification> _specifications;
        public static InstructionSpecification Default { get; } = new InstructionSpecification();
        InstructionSpecification()
            : this(
                SpecificationCandidatesSelector.Default, context => false,
                new InstructionCandidateSpecification(IsTypeSpecification<IProperty>.Default.IsSatisfiedBy)) {}

        public InstructionSpecification(IFactory<object, IEnumerable<object>> candidates,
                                        Func<MemberInfo, bool> defer, params IInstructionCandidateSpecification[] specifications)
        {
            _candidates = candidates;
            _defer = defer;
            _specifications = specifications.ToImmutableList();
        }

        public override bool IsSatisfiedBy(object parameter)
        {
            var candidates = _candidates.Create(parameter);
            foreach (var candidate in candidates)
            {
                foreach (var specification in _specifications)
                {
                    if (specification.Handles(candidate))
                    {
                        var result = specification.IsSatisfiedBy(candidate);
                        return result;
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
            AutoAttributeValueSpecification.Default,
            new InstructionCandidateSpecification(IsPrimitiveSpecification.Default.Adapt().IsSatisfiedBy),
            new InstructionCandidateSpecification(IsPropertyMemberSpecification.Default.Adapt().IsSatisfiedBy)
        ) {}
    }

    public class AutoAttributeWritePlanComposer : DefaultWritePlanComposer
    {
        public new static AutoAttributeWritePlanComposer Default { get; } = new AutoAttributeWritePlanComposer();
        AutoAttributeWritePlanComposer() : base(AutoAttributeSpecification.Default) {}
    }

    class AutoAttributeValueSpecification : MemberValueSpecificationBase<string>
    {
        public static AutoAttributeValueSpecification Default { get; } = new AutoAttributeValueSpecification();
        AutoAttributeValueSpecification() : this(128) {}

        private readonly int _maxLength;

        public AutoAttributeValueSpecification(int maxLength)
        {
            _maxLength = maxLength;
        }

        protected override bool IsSatisfiedBy(MemberContext parameter)
        {
            var value = parameter.Value as string;
            var result = value?.Length < _maxLength;
            return result;
        }
    }

    class AdditionalInstructions : IFactory<IWritePlan, IImmutableList<IWritePlan>>
    {
        public static AdditionalInstructions Default { get; } = new AdditionalInstructions();
        AdditionalInstructions() {}

        public IImmutableList<IWritePlan> Create(IWritePlan parameter) => ImmutableList.Create<IWritePlan>();
    }

    public class DefaultWritePlanComposer : IWritePlanComposer
    {
        public static DefaultWritePlanComposer Default { get; } = new DefaultWritePlanComposer();
        DefaultWritePlanComposer() : this(InstructionSpecification.Default) {}

        private readonly IInstructionSpecification _specification;
        private readonly IFactory<IWritePlan, IImmutableList<IWritePlan>> _additional;

        public DefaultWritePlanComposer(IInstructionSpecification specification) : this(specification, AdditionalInstructions.Default) {}

        public DefaultWritePlanComposer(IInstructionSpecification specification,
                                        IFactory<IWritePlan, IImmutableList<IWritePlan>> additional)
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
                    .Concat(_additional.Create(selector))
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
        protected override IInstruction Plan(Type type) => _builder.For(type);
    }

    abstract class ConditionalWritePlanBase : IWritePlan
    {
        private readonly Func<Type, bool> _specification;

        protected ConditionalWritePlanBase(Func<Type, bool> specification)
        {
            _specification = specification;
        }

        public IInstruction For(Type type) => _specification(type) ? Plan(type) : null;
        protected abstract IInstruction Plan(Type type);
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

    class ObjectMembersWritePlan : IWritePlan
    {
        readonly private static Func<Type, IImmutableList<MemberInfo>> Members = SerializableMembers.Default.Get;

        private readonly IWritePlan _primary;
        private readonly Func<MemberInfo, bool> _deferred;
        private readonly Func<MemberContext, IMemberInstruction> _factory;
        private readonly Func<object, bool> _specification;
        private readonly Func<Type, IImmutableList<MemberInfo>> _members;
        
        public ObjectMembersWritePlan(IWritePlan primary, IInstructionSpecification specification)
            : this(primary, new MemberInstructionFactory(primary, specification).Create, specification.IsSatisfiedBy, specification.Defer, Members) {}

        public ObjectMembersWritePlan(
            IWritePlan primary, 
            Func<MemberContext, IMemberInstruction> factory, 
            Func<object, bool> specification,
            Func<MemberInfo, bool> deferred,
            Func<Type, IImmutableList<MemberInfo>> members
        ) {
            _primary = primary;
            _deferred = deferred;
            _factory = factory;
            _specification = specification;
            _members = members;
        }

        public IInstruction For(Type type)
        {
            var all = _members(type);
            var deferred = all.Where(_deferred).ToImmutableList();
            var members = all.Except(deferred).Select(info => new MemberContext(info)).Select(_factory).ToArray();
            var properties = members.OfType<IPropertyInstruction>().ToImmutableList();
            var contents = members.Except(properties).ToImmutableList();
            
            var body = new CompositeInstruction(
                new CompositeInstruction(properties),
                new EmitDifferentiatingMembersInstruction(type, _factory,
                    new CompositeInstruction(
                        new EmitDeferredMembersInstruction(deferred, _factory,
                                                           new EmitAttachedPropertiesInstruction(_primary,
                                                                                                 _specification)),
                        new CompositeInstruction(contents)
                    )
                )
            );
            var result = new StartNewMembersContextInstruction(all, new EmitWithTypeInstruction(body));
            return result;
        }
    }

    abstract class EmitMembersInstructionBase : DecoratedWriteInstruction
    {
        private readonly Func<MemberContext, IMemberInstruction> _factory;
        
        protected EmitMembersInstructionBase(Func<MemberContext, IMemberInstruction> factory, IInstruction instruction) : base(instruction)
        {
            _factory = factory;
        }

        protected override void Execute(IWriting writing)
        {
            var all = DetermineSet(writing);

            var instructions = all.Select(_factory).ToArray();

            var properties = instructions.OfType<IPropertyInstruction>().ToArray();
            
            foreach (var instruction in properties)
            {
                instruction.Execute(writing);
            }

            base.Execute(writing);

            foreach (var instruction in instructions.Except(properties))
            {
                instruction.Execute(writing);
            }
        }

        protected abstract IImmutableList<MemberContext> DetermineSet(IWriting services);
    }

    class EmitDifferentiatingMembersInstruction : EmitMembersInstructionBase
    {
        private readonly Func<Type, IImmutableList<MemberContext>> _differentiating;

        public EmitDifferentiatingMembersInstruction(Type type, Func<MemberContext, IMemberInstruction> factory,
                                                     IInstruction instruction)
            : this(DifferentiatingDefinitions.Default.Get(type), factory, instruction) {}

        public EmitDifferentiatingMembersInstruction(
            Func<Type, IImmutableList<MemberContext>> differentiating,
            Func<MemberContext, IMemberInstruction> factory,
            IInstruction instruction) : base(factory, instruction)
        {
            _differentiating = differentiating;
        }

        protected override IImmutableList<MemberContext> DetermineSet(IWriting services)
            => _differentiating(services.Current.Instance.GetType());
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

        protected override void Execute(IWriting writing)
        {
            var all = writing.GetProperties();
            var properties = Properties(all).ToArray();
            foreach (var property in properties)
            {
                writing.Emit(property);
            }

            foreach (var content in all.Except(properties))
            {
                new EmitObjectInstruction(content.Name, _primary.For(content.Value.GetType())).Execute(writing);
            }
        }

        IEnumerable<IProperty> Properties(IEnumerable<IProperty> source)
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

    class DifferentiatingMembers : WeakCacheBase<Type, IImmutableList<MemberContext>>
    {
        private readonly IImmutableList<MemberInfo> _definition;
        public DifferentiatingMembers(Type definition) : this(SerializableMembers.Default.Get(definition)) { }
        public DifferentiatingMembers(IImmutableList<MemberInfo> definition)
        {
            _definition = definition;
        }

        protected override IImmutableList<MemberContext> Callback(Type key)
        {
            var list = SerializableMembers.Default.Get(key);
            var result = list.Except(_definition, MemberInfoEqualityComparer.Default).Select( info => new MemberContext(info)).ToImmutableList();
            return result;
        }
    }

    class MemberInfoEqualityComparer : IEqualityComparer<MemberInfo>
    {
        public static MemberInfoEqualityComparer Default { get; } = new MemberInfoEqualityComparer();
        MemberInfoEqualityComparer() {}

        public bool Equals(MemberInfo x, MemberInfo y) => /*x.DeclaringType == y.DeclaringType &&*/ x.Name == y.Name;

        public int GetHashCode(MemberInfo obj) => /*obj.DeclaringType.GetHashCode()*397 ^*/ obj.Name.GetHashCode();
    }

    class DifferentiatingDefinitions : WeakCache<Type, Func<Type, IImmutableList<MemberContext>>>
    {
        public static DifferentiatingDefinitions Default { get; } = new DifferentiatingDefinitions();
        DifferentiatingDefinitions() : base(type => new DifferentiatingMembers(type).Get) {}
    }

    class EmitDeferredMembersInstruction : EmitMembersInstructionBase
    {
        private readonly IImmutableList<MemberInfo> _deferred;

        public EmitDeferredMembersInstruction(
            IImmutableList<MemberInfo> deferred,
            Func<MemberContext, IMemberInstruction> factory,
            IInstruction instruction) : base(factory, instruction)
        {
            _deferred = deferred;
        }

        protected override IImmutableList<MemberContext> DetermineSet(IWriting services) => Yield(services).ToImmutableList();

        private IEnumerable<MemberContext> Yield(IWritingContext context)
        {
            foreach (var member in _deferred)
            {
                yield return MemberContexts.Default.Locate(context.Current.Instance, member);
            }
        }
    }

    public interface IMemberInstruction : IInstruction {}
    public interface IPropertyInstruction : IMemberInstruction {}
    public interface IContentInstruction : IMemberInstruction {}

    public interface IMemberInstructionFactory : IFactory<MemberContext, IMemberInstruction>
    {}

    class MemberInstructionFactory : IMemberInstructionFactory
    {
        private readonly IWritePlan _writePlan;
        private readonly IInstructionSpecification _specification;
        readonly private static StartNewMemberValueContextInstruction PropertyContent =
            new StartNewMemberValueContextInstruction(EmitMemberAsTextInstruction.Default);

        public MemberInstructionFactory(IWritePlan primary, IInstructionSpecification specification)
        {
            _writePlan = primary;
            _specification = specification;
        }

        IInstruction Content(MemberContext member) =>
            new EmitObjectInstruction(
                member.Metadata,
                new StartNewMemberValueContextInstruction(
                    new StartNewContextFromMemberValueInstruction(
                        new DeferredInstruction(
                            new FixedDecoratedWritePlan(_writePlan, member.MemberType).Get
                        )
                    )
                ));

        bool Specification<T>(T parameter) => _specification.IsSatisfiedBy(parameter);

        public IMemberInstruction Create(MemberContext parameter)
        {
            var property = Specification(parameter);
            var content = property ? PropertyContent : Content(parameter);
            var context = new StartNewMemberContextInstruction(parameter.Metadata, content);
            var result = property
                ? (IMemberInstruction) new EmitMemberAsPropertyInstruction(context)
                : new EmitMemberAsContentInstruction(context);
            return result;
        }
    }

    class GeneralObjectWritePlan : ConditionalWritePlan
    {
        public GeneralObjectWritePlan(IWritePlan plan) : base(type => type == typeof(object),
                                                              new FixedWritePlan(
                                                                  new EmitWithTypeInstruction(
                                                                      new EmitGeneralObjectInstruction(plan)))) {}
    }

    class EmitGeneralObjectInstruction : WriteInstructionBase
    {
        private readonly IWritePlan _plan;
        public EmitGeneralObjectInstruction(IWritePlan plan)
        {
            _plan = plan;
        }

        protected override void Execute(IWriting writing)
        {
            var type = writing.Current.Instance.GetType();
            var selected = _plan.For(type);
            selected?.Execute(writing);
        }
    }

    class ObjectWritePlan : ConditionalWritePlanBase
    {
        private readonly IWritePlan _members;

        public ObjectWritePlan(IWritePlan primary, IInstructionSpecification specification) 
            : this(new ObjectMembersWritePlan(primary, specification)) {}

        public ObjectWritePlan(IWritePlan members) : base(type => TypeDefinitionCache.GetDefinition(type).IsObjectToSerialize)
        {
            _members = members;
        }

        protected override IInstruction Plan(Type type) => _members.For(type);

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

        protected override IInstruction Plan(Type type)
        {
            var elementType = ElementTypeLocator.Default.Locate(type);
            // HACK: This should not be necessary.  Consider a <Element exs:Type="[type]" /> or equivalent format for v2.0.
            var provider = elementType.GetTypeInfo().IsInterface
                ? (INameProvider) InstanceTypeNameProvider.Default
                    : new TypeDefinitionNameProvider(elementType);
            var template = new EmitObjectInstruction(provider, _primary.For(elementType));
            var result = new EmitTypeForTemplateInstruction(new EmitEnumerableInstruction(template));
            return result;
        }
    }

    class EmitMemberTypeSpecification : ISpecification<IWritingContext>
    {
        public static EmitMemberTypeSpecification Default { get; } = new EmitMemberTypeSpecification();
        EmitMemberTypeSpecification() {}
        public bool IsSatisfiedBy(IWritingContext parameter)
        {
            var context = parameter.GetContextWithMember().GetValueOrDefault();
            switch (context.State)
            {
                case WriteState.MemberValue:
                    var member = context.Member.GetValueOrDefault();
                    var result = member.IsWritable && member.Value.GetType() != member.MemberType;
                    return result;
            }
            return false;
        }
    }

    class ConditionalWriteInstruction : DecoratedWriteInstruction
    {
        private readonly ISpecification<IWritingContext> _specification;
        
        public ConditionalWriteInstruction(ISpecification<IWritingContext> specification, IInstruction instruction) : base(instruction)
        {
            _specification = specification;
        }

        protected override void Execute(IWriting writing)
        {
            if (_specification.IsSatisfiedBy(writing))
            {
                base.Execute(writing);
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


    class EmitTypeForTemplateInstruction : EmitWithTypeInstruction
    {
        readonly private static IInstruction Specification =
            new ConditionalWriteInstruction(EmitMemberTypeSpecification.Default, EmitCurrentInstanceTypeInstruction.Default);
        public EmitTypeForTemplateInstruction(IInstruction body) : base(Specification, body) {}
    }

    class EmitWithTypeInstruction : CompositeInstruction
    {
        public EmitWithTypeInstruction(IInstruction body) : this(EmitCurrentInstanceTypeInstruction.Default, body) {}
        
        public EmitWithTypeInstruction(IInstruction type, IInstruction body) : base(type, body) {}
    }

    class PrimitiveWritePlan : ConditionalWritePlan
    {
        readonly private static IWritePlan Emit =
            new FixedWritePlan(EmitInstanceAsTextInstruction.Default);
            
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

        protected override IInstruction Plan(Type type)
        {
            var arguments = TypeDefinitionCache.GetDefinition(type).GenericArguments;
            if (arguments.Length != 2)
            {
                throw new InvalidOperationException(
                          $"Attempted to write type '{type}' as a dictionary, but it does not have enough generic arguments.");
            }
            var keys = new EmitObjectInstruction(ExtendedXmlSerializer.Key, _builder.For(arguments[0]));
            var values = new EmitObjectInstruction(ExtendedXmlSerializer.Value, _builder.For(arguments[1]));
            var result = new EmitTypeForTemplateInstruction(new EmitDictionaryInstruction(keys, values));
            return result;
        }
    }
}