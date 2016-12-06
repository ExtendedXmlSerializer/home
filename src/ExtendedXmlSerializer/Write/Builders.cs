using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWritePlanComposer
    {
        IWritePlan Compose();
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

            if (parameter is MemberContext)
            {
                var context = (MemberContext) parameter;
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
        public InstructionCandidateSpecification(ISpecification<T> specification)
            : base(o => o is T, specification.Adapt().IsSatisfiedBy) {}
    }

    class InstructionCandidateSpecification : DelegatedSpecification<object>, IInstructionCandidateSpecification
    {
        private readonly Func<object, bool> _handles;

        public InstructionCandidateSpecification(Func<object, bool> specification) : this(specification, o => true) {}

        public InstructionCandidateSpecification(Func<object, bool> handles, Func<object, bool> specification)
            : base(specification)
        {
            _handles = handles;
        }

        public virtual bool Handles(object candidate) => _handles(candidate);
    }

    abstract class MemberValueSpecificationBase<T> : InstructionCandidateSpecificationBase<MemberContext>
    {
        public override bool Handles(object candidate) =>
            base.Handles(candidate) && ((MemberContext) candidate).Value is T;
    }

    abstract class MemberContextSpecificationBase<T> : InstructionCandidateSpecificationBase<MemberContext>
    {
        public override bool Handles(object candidate) =>
            base.Handles(candidate) && typeof(T).IsAssignableFrom(((MemberContext) candidate).MemberType);
    }

    abstract class InstructionCandidateSpecificationBase<T> : SpecificationAdapterBase<T>,
                                                              IInstructionCandidateSpecification
    {
        public virtual bool Handles(object candidate) => candidate is T;
    }

    class DefaultInstructionSpecification : InstructionSpecificationBase
    {
        private readonly IParameterizedSource<object, IEnumerable<object>> _candidates;
        private readonly Func<MemberInfo, bool> _defer;
        private readonly IImmutableList<IInstructionCandidateSpecification> _specifications;
        public static DefaultInstructionSpecification Default { get; } = new DefaultInstructionSpecification();

        DefaultInstructionSpecification()
            : this(
                SpecificationCandidatesSelector.Default, context => false,
                new InstructionCandidateSpecification(IsTypeSpecification<IProperty>.Default.IsSatisfiedBy)) {}

        public DefaultInstructionSpecification(IParameterizedSource<object, IEnumerable<object>> candidates,
                                               Func<MemberInfo, bool> defer,
                                               params IInstructionCandidateSpecification[] specifications)
        {
            _candidates = candidates;
            _defer = defer;
            _specifications = specifications.ToImmutableList();
        }

        public override bool IsSatisfiedBy(object parameter)
        {
            var candidates = _candidates.Get(parameter);
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

    class AutoAttributeSpecification : DefaultInstructionSpecification
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

    /*public class AutoAttributePlanSelector : PlanSelector
    {
        public new static AutoAttributePlanSelector Default { get; } = new AutoAttributePlanSelector();
        AutoAttributePlanSelector() : base(AutoAttributeSpecification.Default) {}
    }*/

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

    /*public class EnableExtensionPlanAlteration : IAlteration<IWritePlan>
    {
        private readonly IWritingExtension _extension;

        public EnableExtensionPlanAlteration(IWritingExtension extension)
        {
            _extension = extension;
        }

        public IWritePlan Get(IWritePlan parameter) => new ExtensionEnabledWritePlan(parameter, _extension);
    }*/

/*
                public class ExtensionEnabledPlanSelector : IPlanSelector
                {
                    private readonly IPlanSelector _selector;
                    private readonly IWritingExtension _extension;
            
                    public ExtensionEnabledPlanSelector(IPlanSelector selector, IWritingExtension extension)
                    {
                        _selector = selector;
                        _extension = extension;
                    }
            
                    public IEnumerable<IWritePlan> Get(IWritePlan parameter)
                    {
                        var items = _selector.Get(parameter);
                        foreach (var plan in items)
                        {
                            yield return new ExtensionEnabledWritePlan(plan, _extension);
                        }
                    }
                }
            */

    public interface IPlanSelector : IParameterizedSource<IWritePlan, IEnumerable<IWritePlan>> {}

    public class DefaultPlanSelector : IPlanSelector
    {
        private readonly IInstructionSpecification _specification;
        private readonly IEnumerableInstructionFactory _enumerable;

        public DefaultPlanSelector(IInstructionSpecification specification, IEnumerableInstructionFactory enumerable)
        {
            _specification = specification;
            _enumerable = enumerable;
        }

        public IEnumerable<IWritePlan> Get(IWritePlan parameter)
        {
            yield return PrimitiveWritePlan.Default;
            yield return new DefaultDictionaryWritePlan(parameter);
            yield return new EnumerableWritePlan(parameter, _enumerable);
            yield return new ObjectWritePlan(new DefaultMembersBodyFactory(new MembersBodyFactory(parameter, _specification, new MemberInstructionFactory(parameter, _specification))));
            yield return new DefaultGeneralObjectWritePlan(parameter);
        }
    }

    public class PlanSelector : IPlanSelector
    {
        private readonly IInstructionSpecification _specification;
        private readonly IEnumerableInstructionFactory _enumerable;

        public PlanSelector(IInstructionSpecification specification, IEnumerableInstructionFactory enumerable)
        {
            _specification = specification;
            _enumerable = enumerable;
        }

        public IEnumerable<IWritePlan> Get(IWritePlan parameter)
        {
            yield return PrimitiveWritePlan.Default;
            yield return new DictionaryWritePlan(parameter);
            yield return new EnumerableWritePlan(parameter, _enumerable);
            yield return new ObjectWritePlan(new MembersBodyFactory(parameter, _specification, new MemberInstructionFactory(parameter, _specification)));
            yield return new GeneralObjectWritePlan(parameter);
        }
    }

    class CacheWritePlanAlteration : IAlteration<IWritePlan>
    {
        public static CacheWritePlanAlteration Default { get; } = new CacheWritePlanAlteration();
        CacheWritePlanAlteration() {}

        public IWritePlan Get(IWritePlan parameter) => new CachedWritePlan(parameter);
    }

    public class WritePlanComposer : IWritePlanComposer
    {
        private readonly IAlteration<IWritePlan> _alteration;
        private readonly IPlanSelector _selector;

        public WritePlanComposer(IPlanSelector selector) : this(CacheWritePlanAlteration.Default, selector) {}

        public WritePlanComposer(IAlteration<IWritePlan> alteration, IPlanSelector selector)
        {
            _alteration = alteration;
            _selector = selector;
        }

        public IWritePlan Compose()
        {
            var plans = new OrderedSet<IWritePlan>();
            var select = _alteration.Get(new SelectFirstAssignedWritePlan(plans));
            foreach (var plan in _selector.Get(select))
            {
                plans.Add(plan);
            }

            var result = new RootWritePlan(select);
            return result;
        }
    }

    class DecoratedWritePlan : IWritePlan
    {
        private readonly IWritePlan _plan;

        public DecoratedWritePlan(IWritePlan plan)
        {
            _plan = plan;
        }

        public virtual IInstruction For(Type type) => _plan.For(type);
    }

    class FixedDecoratedWritePlan : IWritePlan
    {
        private readonly IWritePlan _builder;
        private readonly Type _type;

        public FixedDecoratedWritePlan(IWritePlan builder, Type type)
        {
            _builder = builder;
            _type = type;
        }

        public IInstruction For(Type type) => Get();

        public IInstruction Get() => _builder.For(_type);
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

    class InstanceMembersWritePlan : IWritePlan
    {
        readonly private static Func<Type, IImmutableList<MemberInfo>> Members = SerializableMembers.Default.Get;


        private readonly Func<Type, IImmutableList<MemberInfo>> _members;
        private readonly IMembersBodyFactory _body;

        public InstanceMembersWritePlan(IMembersBodyFactory body) : this(Members, body) {}

        public InstanceMembersWritePlan(Func<Type, IImmutableList<MemberInfo>> members, IMembersBodyFactory body)
        {
            _members = members;
            _body = body;
        }

        public IInstruction For(Type type)
        {
            var members = _members(type);
            var body = _body.Create(type, members);
            var result = new StartNewMembersContextInstruction(members, body);
            return result;
        }
    }

    public interface IMembersBodyFactory
    {
        IInstruction Create(Type type, IImmutableList<MemberInfo> members);
    }

    class DefaultMembersBodyFactory : IMembersBodyFactory
    {
        private readonly IMembersBodyFactory _inner;

        public DefaultMembersBodyFactory(IMembersBodyFactory inner)
        {
            _inner = inner;
        }

        public IInstruction Create(Type type, IImmutableList<MemberInfo> members)
            => new EmitWithTypeInstruction(_inner.Create(type, members));
    }

    class MembersBodyFactory : IMembersBodyFactory
    {
        private readonly IWritePlan _primary;
        private readonly Func<MemberInfo, bool> _deferred;
        private readonly Func<MemberContext, IMemberInstruction> _factory;
        private readonly Func<object, bool> _specification;

        public MembersBodyFactory(IWritePlan primary, IInstructionSpecification specification, IMemberInstructionFactory factory)
            : this(
                primary, factory.Get, specification.IsSatisfiedBy,
                specification.Defer) {}

        public MembersBodyFactory(
            IWritePlan primary, Func<MemberContext, IMemberInstruction> factory,
            Func<object, bool> specification, Func<MemberInfo, bool> deferred)
        {
            _primary = primary;
            _deferred = deferred;
            _factory = factory;
            _specification = specification;
        }

        public IInstruction Create(Type type, IImmutableList<MemberInfo> members)
        {
            var deferred = members.Where(_deferred).ToImmutableList();
            var instructions = members.Except(deferred).Select(info => new MemberContext(info)).Select(_factory).ToArray();
            var properties = instructions.OfType<IPropertyInstruction>().ToImmutableList();
            var contents = instructions.Except(properties).ToImmutableList();

            var result =
                new CompositeInstruction(
                    new CompositeInstruction(properties),
                    new EmitDifferentiatingMembersInstruction(type, _factory,
                                                              new CompositeInstruction(
                                                                  new EmitDeferredMembersInstruction(deferred, _factory,
                                                                                                     new EmitAttachedPropertiesInstruction
                                                                                                     (_primary,
                                                                                                      _specification)),
                                                                  new CompositeInstruction(contents)
                                                              )
                    )
                );
            return result;
        }
    }

    class DifferentiatingMembers : WeakCacheBase<Type, IImmutableList<MemberContext>>
    {
        private readonly IImmutableList<MemberInfo> _definition;
        public DifferentiatingMembers(Type definition) : this(SerializableMembers.Default.Get(definition)) {}

        public DifferentiatingMembers(IImmutableList<MemberInfo> definition)
        {
            _definition = definition;
        }

        protected override IImmutableList<MemberContext> Callback(Type key)
        {
            var list = SerializableMembers.Default.Get(key);
            var result = list.Except(_definition, MemberInfoEqualityComparer.Default).Select(info => new MemberContext(info)).ToImmutableList();
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

    public interface IMemberInstructionFactory : IParameterizedSource<MemberContext, IMemberInstruction> {}

    class MemberInstructionFactory : IMemberInstructionFactory
    {
        readonly private static IInstruction Property = new StartNewMemberValueContextInstruction(EmitMemberAsTextInstruction.Default);
        private readonly IWritePlan _writePlan;
        private readonly IInstructionSpecification _specification;
        readonly private IInstruction _property;

        public MemberInstructionFactory(IWritePlan plan, IInstructionSpecification specification) : this(plan, specification, Property) {}

        public MemberInstructionFactory(IWritePlan plan, IInstructionSpecification specification, IInstruction property)
        {
            _writePlan = plan;
            _specification = specification;
            _property = property;
        }

        IInstruction Content(MemberContext member) =>
            new EmitInstanceInstruction(
                member.Metadata,
                new StartNewMemberValueContextInstruction(
                    new StartNewContextFromMemberValueInstruction(
                        new DeferredInstruction(
                            new FixedDecoratedWritePlan(_writePlan, member.MemberType).Get
                        )
                    )
                )
            );

        bool Specification<T>(T parameter) => _specification.IsSatisfiedBy(parameter);

        public IMemberInstruction Get(MemberContext parameter)
        {
            var property = Specification(parameter);
            var content = property ? _property : Content(parameter);
            var context = new StartNewMemberContextInstruction(parameter.Metadata, content);
            var result = property
                ? (IMemberInstruction) new EmitMemberAsPropertyInstruction(context)
                : new EmitMemberAsContentInstruction(context);
            return result;
        }
    }

    class GeneralObjectWritePlan : ConditionalWritePlan
    {
        public GeneralObjectWritePlan(IWritePlan plan) : base(
            type => type == typeof(object),
            new FixedWritePlan(new EmitGeneralObjectInstruction(plan))) {}
    }

    class DefaultGeneralObjectWritePlan : ConditionalWritePlan
    {
        public DefaultGeneralObjectWritePlan(IWritePlan plan) : base(
            type => type == typeof(object),
            new FixedWritePlan(
                new EmitWithTypeInstruction(
                    new EmitGeneralObjectInstruction(plan)))) {}
    }

    class ObjectWritePlan : ConditionalWritePlanBase
    {
        private readonly IWritePlan _members;

        public ObjectWritePlan(IMembersBodyFactory body)
            : this(new InstanceMembersWritePlan(body)) {}

        public ObjectWritePlan(IWritePlan members) : base(type => TypeDefinitionCache.GetDefinition(type).IsObjectToSerialize)
        {
            _members = members;
        }

        protected override IInstruction Plan(Type type) => _members.For(type);
    }

    class RootWritePlan : IWritePlan
    {
        private readonly IWritePlan _plan;

        public RootWritePlan(IWritePlan plan)
        {
            _plan = plan;
        }

        public IInstruction For(Type type)
        {
            var content = _plan.For(type);
            if (content == null)
            {
                throw new InvalidOperationException($"Could not find instruction for type '{type}'");
            }
            var result = new StartNewContextFromRootInstruction(new EmitInstanceInstruction(type, content));
            return result;
        }
    }

    class CachedWritePlan : WeakCache<Type, IInstruction>, IWritePlan
    {
        public CachedWritePlan(IWritePlan inner) : base(inner.For) {}

        public IInstruction For(Type type) => Get(type);
    }

    /*class DefaultEnumerableWritePlan : ConditionalWritePlanBase
    {
        readonly private static Func<Type, bool> Specification = IsSatisfiedBy;

        private readonly IWritePlan _plan;

        public DefaultEnumerableWritePlan(IWritePlan plan) : base(Specification)
        {
            _plan = plan;
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
            var provider = elementType.GetTypeInfo().IsInterface
                ? (IElementProvider) InstanceTypeNameProvider.Default
                    : new TypeDefinitionElementProvider(elementType);
            var template = new EmitInstanceInstruction(provider, _plan.For(elementType));
            var result = new EmitTypeForTemplateInstruction(new EmitEnumerableInstruction(template));
            return result;
        }
    }*/

    class IsEnumerableTypeSpecification : ISpecification<Type>
    {
        public static IsEnumerableTypeSpecification Default { get; } = new IsEnumerableTypeSpecification();
        IsEnumerableTypeSpecification() {}

        public bool IsSatisfiedBy(Type parameter)
        {
            var definition = TypeDefinitionCache.GetDefinition(parameter);
            var result = definition.IsArray || definition.IsEnumerable;
            return result;
        }
    }

    public interface IEnumerableInstructionFactory
    {
        IInstruction Create(Type elementType, IInstruction instruction);
    }

    class EnumerableInstructionFactory : IEnumerableInstructionFactory
    {
        public static EnumerableInstructionFactory Default { get; } = new EnumerableInstructionFactory();
        EnumerableInstructionFactory() {}

        public IInstruction Create(Type elementType, IInstruction instruction)
        {
            var template = new EmitInstanceInstruction(InstanceTypeNameProvider.Default, instruction);
            var result = new EmitEnumerableInstruction(template);
            return result;
        }
    }

    class DefaultEnumerableInstructionFactory : IEnumerableInstructionFactory
    {
        public static DefaultEnumerableInstructionFactory Default { get; } = new DefaultEnumerableInstructionFactory();
        DefaultEnumerableInstructionFactory() {}

        public IInstruction Create(Type elementType, IInstruction instruction)
        {
            var provider = elementType.GetTypeInfo().IsInterface
                ? (IElementProvider) InstanceTypeNameProvider.Default
                : new TypeDefinitionElementProvider(elementType);
            var template = new EmitInstanceInstruction(provider, instruction);
            var result = new EmitTypeForTemplateInstruction(new EmitEnumerableInstruction(template));
            return result;
        }
    }

    class EnumerableWritePlan : ConditionalWritePlanBase
    {
        private readonly IWritePlan _plan;
        private readonly IEnumerableInstructionFactory _factory;

        public EnumerableWritePlan(IWritePlan plan, IEnumerableInstructionFactory factory) : this(IsEnumerableTypeSpecification.Default.IsSatisfiedBy, plan, factory) {}

        public EnumerableWritePlan(Func<Type, bool> specification, IWritePlan plan, IEnumerableInstructionFactory factory) : base(specification)
        {
            _plan = plan;
            _factory = factory;
        }

        protected override IInstruction Plan(Type type)
        {
            var elementType = ElementTypeLocator.Default.Locate(type);
            var instruction = _plan.For(elementType);
            var result = _factory.Create(elementType, instruction);
            return result;
        }
    }

    class EmitMemberTypeSpecification : ISpecification<IWritingContext>
    {
        public static EmitMemberTypeSpecification Default { get; } = new EmitMemberTypeSpecification();
        EmitMemberTypeSpecification() {}

        public bool IsSatisfiedBy(IWritingContext parameter)
        {
            var context = parameter.GetMemberContext().GetValueOrDefault();
            switch (context.State)
            {
                case WriteState.MemberValue:
                    var member = context.Member.GetValueOrDefault();
                    var result = member.IsWritable && member.Value.GetType() != member.MemberType;
                    return result;
            }
            var array = parameter.GetArrayContext();
            if (array != null)
            {
                var elementType = ElementTypeLocator.Default.Locate(array.Value.Instance.GetType());
                var result = parameter.Current.Instance.GetType() != elementType;
                return result;
            }

            var dictionary = parameter.GetDictionaryContext();
            if (dictionary != null)
            {
                var type = TypeDefinitionCache.GetDefinition(dictionary.Value.Instance.GetType()).GenericArguments[1];
                var result = parameter.Current.Instance.GetType() != type;
                return result;
            }
            return false;
        }
    }

    class SelectFirstAssignedWritePlan : IWritePlan
    {
        readonly ICollection<IWritePlan> _providers;

        public SelectFirstAssignedWritePlan(ICollection<IWritePlan> providers)
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

    class PrimitiveWritePlan : ConditionalWritePlan
    {
        readonly private static IWritePlan Emit =
            new FixedWritePlan(EmitInstanceAsTextInstruction.Default);

        public static PrimitiveWritePlan Default { get; } = new PrimitiveWritePlan();
        PrimitiveWritePlan() : base(type => TypeDefinitionCache.GetDefinition(type).IsPrimitive, Emit) {}
    }

    class DefaultDictionaryWritePlan : DictionaryWritePlan
    {
        public DefaultDictionaryWritePlan(IWritePlan plan) : base(plan) {}

        protected override IInstruction Plan(Type type) => new EmitTypeForTemplateInstruction(base.Plan(type));
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
            var keys = new EmitInstanceInstruction((ns, o) => new DictionaryKeyElement(ns), _builder.For(arguments[0]));
            var values = new EmitInstanceInstruction((ns, o) => new DictionaryValueElement(ns), _builder.For(arguments[1]));
            var result = new EmitDictionaryInstruction(keys, values);
            return result;
        }
    }
}