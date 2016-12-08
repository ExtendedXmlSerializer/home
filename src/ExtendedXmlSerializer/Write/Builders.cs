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

    public class DefaultPlanSelector : PlanSelector
    {
        /*private readonly IInstructionSpecification _specification;
        private readonly IEnumerableInstructionFactory _enumerable;*/

        public DefaultPlanSelector(IInstructionSpecification specification, IEnumerableInstructionFactory enumerable)
            : base(specification, enumerable, EmitTypeInstruction.Default) {}

        /*public IEnumerable<IPlan> Get(IPlan parameter)
        {
            yield return PrimitiveWritePlan.Default;
            yield return new DictionaryWritePlan(parameter, EmitTypeInstruction.Default);
            yield return new EnumerableWritePlan(parameter, _enumerable);
            yield return new InstanceWritePlan(parameter, _specification, EmitTypeInstruction.Default, new MemberInstructionFactory(parameter, _specification))
                ;
            yield return new ObjectWritePlan(parameter, EmitTypeInstruction.Default);
        }*/
    }

    public interface IPlanSelector : IParameterizedSource<IPlan, IEnumerable<IPlan>> {}
    public class PlanSelector : IPlanSelector
    {
        private readonly IInstructionSpecification _specification;
        private readonly IEnumerableInstructionFactory _enumerable;
        private readonly IInstruction _emitType;

        public PlanSelector(IInstructionSpecification specification, IEnumerableInstructionFactory enumerable, IInstruction emitType)
        {
            _specification = specification;
            _enumerable = enumerable;
            _emitType = emitType;
        }

        public IEnumerable<IPlan> Get(IPlan parameter)
        {
            yield return PrimitiveWritePlan.Default;
            yield return new DictionaryWritePlan(parameter, _emitType);
            yield return new EnumerableWritePlan(parameter, _enumerable);
            yield return
                new InstanceWritePlan(parameter, _specification, _emitType,
                                      new MemberInstructionFactory(parameter, _specification));
            yield return new ObjectWritePlan(parameter, _emitType);
        }
    }

    public class PlanMaker : IPlanMaker
    {
        private readonly IAlteration<IPlan> _alteration;
        private readonly IPlanSelector _selector;

        public PlanMaker(IPlanSelector selector) : this(CachePlanAlteration.Default, selector) {}

        public PlanMaker(IAlteration<IPlan> alteration, IPlanSelector selector)
        {
            _alteration = alteration;
            _selector = selector;
        }

        public IPlan Make()
        {
            var plans = new OrderedSet<IPlan>();
            var select = _alteration.Get(new SelectFirstAssignedPlan(plans));
            foreach (var plan in _selector.Get(select))
            {
                plans.Add(plan);
            }

            var result = new RootWritePlan(select);
            return result;
        }
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

    /*public interface IMembersBodyFactory
    {
        IInstruction Create(Type type, IImmutableList<MemberInfo> members);
    }*/

    /*class DefaultMembersBodyFactory : IMembersBodyFactory
    {
        private readonly IMembersBodyFactory _inner;

        public DefaultMembersBodyFactory(IMembersBodyFactory inner)
        {
            _inner = inner;
        }

        public IInstruction Create(Type type, IImmutableList<MemberInfo> members)
            => new EmitWithTypeInstruction(_inner.Create(type, members));
    }*/

    /*class EmitTypedMembersBodyFactory : IMembersBodyFactory
    {
        private readonly IMembersBodyFactory _inner;

        public EmitTypedMembersBodyFactory(IMembersBodyFactory inner)
        {
            _inner = inner;
        }

        public IInstruction Create(Type type, IImmutableList<MemberInfo> members)
            => new Instruction(_inner.Create(type, members));

        class Instruction : WriteInstructionBase
        {
            private readonly IInstruction _body;
            private readonly IInstruction _wrapped;

            public Instruction(IInstruction body) : this(body, new EmitInstanceInstruction(InstanceTypeNameProvider.Default, body)) {}

            public Instruction(IInstruction body, IInstruction wrapped)
            {
                _body = body;
                _wrapped = wrapped;
            }

            protected override void Execute(IWriting services)
            {
                var instruction = services.Current.Instance == services.Current.Root ? _body : _wrapped;
                instruction.Execute(services);
            }
        }
    }*/

    /*class MembersBodyFactory : IMembersBodyFactory
    {
        
    }*/

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
        private readonly IPlan _plan;
        private readonly IInstructionSpecification _specification;
        readonly private IInstruction _property;

        public MemberInstructionFactory(IPlan plan, IInstructionSpecification specification) : this(plan, specification, Property) {}

        public MemberInstructionFactory(IPlan plan, IInstructionSpecification specification, IInstruction property)
        {
            _plan = plan;
            _specification = specification;
            _property = property;
        }

        IInstruction Content(MemberContext member) =>
            new EmitInstanceInstruction(
                member,
                new StartNewMemberValueContextInstruction(
                    new StartNewContextFromMemberValueInstruction(
                        new DeferredInstruction(
                            new FixedDecoratedPlan(_plan, member.MemberType).Get
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

    class ObjectWritePlan : ConditionalPlan
    {
        public ObjectWritePlan(IPlan plan, IInstruction emitType) : base(
            type => type == typeof(object),
            new FixedPlan(new EmitWithTypeInstruction(emitType, new EmitGeneralObjectInstruction(plan)))) {}
    }

    /*class DefaultGeneralObjectWritePlan : ConditionalWritePlan
    {
        public DefaultGeneralObjectWritePlan(IWritePlan plan) : base(
            type => type == typeof(object),
            new FixedWritePlan(
                new EmitWithTypeInstruction(
                    new EmitGeneralObjectInstruction(plan)))) {}
    }*/

    class InstanceWritePlan : ConditionalPlanBase
    {
        readonly private static Func<Type, IImmutableList<MemberInfo>> Members = SerializableMembers.Default.Get;
        private readonly IInstruction _emitType;
        private readonly Func<Type, IImmutableList<MemberInfo>> _members;
        private readonly IPlan _primary;
        private readonly Func<MemberInfo, bool> _deferred;
        private readonly Func<MemberContext, IMemberInstruction> _factory;
        private readonly Func<object, bool> _specification;

        public InstanceWritePlan(IPlan primary, IInstructionSpecification specification, IInstruction emitType, IMemberInstructionFactory factory)
            : this(primary, emitType, factory.Get, specification.IsSatisfiedBy, specification.Defer, Members) {}

        public InstanceWritePlan(
            IPlan primary, IInstruction emitType, Func<MemberContext, IMemberInstruction> factory,
            Func<object, bool> specification, Func<MemberInfo, bool> deferred, Func<Type, IImmutableList<MemberInfo>> members) : base(type => TypeDefinitionCache.GetDefinition(type).IsObjectToSerialize)
        {
            _primary = primary;
            _emitType = emitType;
            _deferred = deferred;
            _members = members;
            _factory = factory;
            _specification = specification;
        }

        protected override IInstruction Plan(Type type)
        {
            var members = _members(type);
            var deferred = members.Where(_deferred).ToImmutableList();
            var instructions = members.Except(deferred).Select(info => new MemberContext(info)).Select(_factory).ToArray();
            var properties = instructions.OfType<IPropertyInstruction>().ToImmutableList();
            var contents = instructions.Except(properties).ToImmutableList();
            var body = new CompositeInstruction(
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

            var typed = new EmitWithTypeInstruction(_emitType, body);
            var result = new StartNewMembersContextInstruction(members, typed);
            return result;
        }
    }

    class RootWritePlan : IPlan
    {
        private readonly IPlan _plan;

        public RootWritePlan(IPlan plan)
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
            var result = new StartNewContextFromRootInstruction(new EmitRootInstruction(content));
            return result;
        }
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
        private readonly IInstruction _emitType;
        /*public static EnumerableInstructionFactory Default { get; } = new EnumerableInstructionFactory();
        EnumerableInstructionFactory() {}*/

        public EnumerableInstructionFactory(IInstruction emitType)
        {
            _emitType = emitType;
        }

        public IInstruction Create(Type elementType, IInstruction instruction)
        {
            var template = new EmitInstanceInstruction(InstanceTypeNameProvider.Default, instruction);
            var result = new EmitTypeForTemplateInstruction(_emitType, new EmitEnumerableInstruction(template));
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

    class EnumerableWritePlan : ConditionalPlanBase
    {
        private readonly IPlan _plan;
        private readonly IEnumerableInstructionFactory _factory;

        public EnumerableWritePlan(IPlan plan, IEnumerableInstructionFactory factory) : this(IsEnumerableTypeSpecification.Default.IsSatisfiedBy, plan, factory) {}

        public EnumerableWritePlan(Func<Type, bool> specification, IPlan plan, IEnumerableInstructionFactory factory) : base(specification)
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

    class PrimitiveWritePlan : ConditionalPlan
    {
        readonly private static IPlan Emit =
            new FixedPlan(EmitInstanceAsTextInstruction.Default);

        public static PrimitiveWritePlan Default { get; } = new PrimitiveWritePlan();
        PrimitiveWritePlan() : base(IsPrimitiveSpecification.Default.IsSatisfiedBy, Emit) {}
    }

    /*class CanSerializeSpecification : ISpecification<Type>
    {
        public static CanSerializeSpecification Default { get; } = new CanSerializeSpecification();
        CanSerializeSpecification() {}

        public bool IsSatisfiedBy(Type parameter)
        {
            var definition = TypeDefinitionCache.GetDefinition(parameter);
            var result = definition.IsPrimitive || definition.IsArray || definition.IsEnumerable ||
                         definition.IsObjectToSerialize;
            return result;
        }
    }*/

    /*class DefaultDictionaryWritePlan : DictionaryWritePlan
    {
        public DefaultDictionaryWritePlan(IWritePlan plan) : base(plan) {}

        protected override IInstruction Plan(Type type) => new EmitTypeForTemplateInstruction(base.Plan(type));
    }*/

    class DictionaryWritePlan : ConditionalPlanBase
    {
        private readonly IPlan _builder;
        private readonly IInstruction _emitType;

        public DictionaryWritePlan(IPlan builder, IInstruction emitType) : base(type => TypeDefinitionCache.GetDefinition(type).IsDictionary)
        {
            _builder = builder;
            _emitType = emitType;
        }

        protected override IInstruction Plan(Type type)
        {
            var arguments = TypeDefinitionCache.GetDefinition(type).GenericArguments;
            if (arguments.Length != 2)
            {
                throw new InvalidOperationException(
                          $"Attempted to write type '{type}' as a dictionary, but it does not have enough generic arguments.");
            }
            var keys = new EmitInstanceInstruction(new ApplicationElementProvider((ns, o) => new DictionaryKeyElement(ns)), _builder.For(arguments[0]));
            var values = new EmitInstanceInstruction(new ApplicationElementProvider((ns, o) => new DictionaryValueElement(ns)), _builder.For(arguments[1]));
            var result = new EmitTypeForTemplateInstruction(_emitType, new EmitDictionaryInstruction(keys, values));
            return result;
        }
    }
}