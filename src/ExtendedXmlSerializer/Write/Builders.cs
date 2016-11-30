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
    public interface IInstructionCompiler
    {
        IInstructions Compile();
    }

    class MemberInfoNameProvider : FixedNameProvider
    {
        public MemberInfoNameProvider(MemberInfo member) 
            : base(member.GetCustomAttribute<XmlAttributeAttribute>()?.AttributeName ?? member.Name) {}
    }

    public interface IInstructionSpecification : ISpecification<Type>, ISpecification<MemberInfo>, ISpecification<WriteContext>
    {
        bool Defer(MemberInfo member);
    }

    abstract class InstructionSpecificationBase : IInstructionSpecification
    {
        public abstract bool IsSatisfiedBy(Type parameter);

        public abstract bool IsSatisfiedBy(MemberInfo parameter);

        public abstract bool IsSatisfiedBy(WriteContext parameter);

        public virtual bool Defer(MemberInfo member) => false;
    }

    class InstructionSpecification : InstructionSpecificationBase
    {
        public static InstructionSpecification Default { get; } = new InstructionSpecification();
        InstructionSpecification() : this(context => false) {}

        private readonly Func<Type, bool> _isPropertyType;
        private readonly Func<MemberInfo, bool> _isPropertyMember;
        private readonly Func<WriteContext, bool> _isPropertyDeferred;
        private readonly Func<MemberInfo, bool> _shouldDefer;

        public InstructionSpecification(Func<WriteContext, bool> isPropertyDeferred)
            : this(info => false, isPropertyDeferred) {}

        public InstructionSpecification(Func<MemberInfo, bool> isProperty, Func<WriteContext, bool> isPropertyDeferred)
            : this(isProperty, isPropertyDeferred, info => false) {}

        public InstructionSpecification(Func<MemberInfo, bool> isPropertyMember,
                                        Func<WriteContext, bool> isPropertyDeferred, Func<MemberInfo, bool> shouldDefer)
            : this(type => false, isPropertyMember, isPropertyDeferred, shouldDefer) {}

        public InstructionSpecification(Func<Type, bool> isPropertyType, Func<MemberInfo, bool> isPropertyMember,
                                        Func<WriteContext, bool> isPropertyDeferred, Func<MemberInfo, bool> shouldDefer)
        {
            _isPropertyType = isPropertyType;
            _isPropertyMember = isPropertyMember;
            _isPropertyDeferred = isPropertyDeferred;
            _shouldDefer = shouldDefer;
        }

        public override bool IsSatisfiedBy(Type parameter) => _isPropertyType(parameter);
        public override bool IsSatisfiedBy(MemberInfo parameter) => _isPropertyMember(parameter) || IsSatisfiedBy(parameter.GetMemberType());
        public override bool IsSatisfiedBy(WriteContext parameter) => _isPropertyDeferred(parameter) || IsSatisfiedBy(parameter.Member);
        public override bool Defer(MemberInfo member) => _shouldDefer(member);
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
            IsPrimitiveSpecification.Default.IsSatisfiedBy,
            IsPropertyMemberSpecification.Default.IsSatisfiedBy,
            AutoAttributeValueSpecification.Default.IsSatisfiedBy,
            info => info.GetMemberType() == typeof(string)
        ) {}
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

    class AdditionalInstructions : IParameterizedSource<IInstructions, IImmutableList<IInstructions>>
    {
        public static AdditionalInstructions Default { get; } = new AdditionalInstructions();
        AdditionalInstructions() {}

        public IImmutableList<IInstructions> Get(IInstructions parameter) => ImmutableList.Create<IInstructions>();
    }

    class AutoAttributeInstructionCompiler : DefaultInstructionCompiler
    {
        public new static AutoAttributeInstructionCompiler Default { get; } = new AutoAttributeInstructionCompiler();
        AutoAttributeInstructionCompiler() : base(AutoAttributeSpecification.Default) {}
    }

    class DefaultInstructionCompiler : IInstructionCompiler
    {
        public static DefaultInstructionCompiler Default { get; } = new DefaultInstructionCompiler();
        DefaultInstructionCompiler() : this(InstructionSpecification.Default) {}

        private readonly IInstructionSpecification _specification;
        private readonly IParameterizedSource<IInstructions, IImmutableList<IInstructions>> _additional;

        public DefaultInstructionCompiler(IInstructionSpecification specification) : this(specification, AdditionalInstructions.Default) {}

        public DefaultInstructionCompiler(IInstructionSpecification specification,
                                          IParameterizedSource<IInstructions, IImmutableList<IInstructions>> additional)
        {
            _specification = specification;
            _additional = additional;
        }

        public IInstructions Compile()
        {
            var collection = new List<IInstructions>();
            var selector = new CachedInstructions(new FirstAssignedInstructions(collection));
            collection.AddRange(
                new IInstructions[]
                    {
                        ContentInstructions.Default,
                        new DictionaryInstructions(selector),
                        new EnumerableInstructions(selector),
                        new ObjectInstructions(selector, _specification)
                    }
                    .Concat(_additional.Get(selector))
            );
            var result = new CachedInstructions(new RootInstructions(selector));
            return result;
        }
    }


    public interface IInstructions
    {
        IInstruction For(Type type);
    }

    class ConditionalInstructions : ConditionalInstructionsBase
    {
        private readonly IInstructions _builder;
        public ConditionalInstructions(Func<Type, bool> specification, IInstructions builder) : base(specification)
        {
            _builder = builder;
        }
        protected override IInstruction PerformBuild(Type type) => _builder.For(type);
    }

    abstract class ConditionalInstructionsBase : IInstructions
    {
        private readonly Func<Type, bool> _specification;

        protected ConditionalInstructionsBase(Func<Type, bool> specification)
        {
            _specification = specification;
        }

        public IInstruction For(Type type) => _specification(type) ? PerformBuild(type) : null;
        protected abstract IInstruction PerformBuild(Type type);
    }

    class SerializableMembers : WeakCacheBase<Type, IImmutableSet<MemberInfo>>
    {
        public static SerializableMembers Default { get; } = new SerializableMembers();
        SerializableMembers() : this(CanSerializeSpecification.Default.IsSatisfiedBy) {}

        private readonly Func<TypeDefinition, bool> _serializable;

        public SerializableMembers(Func<TypeDefinition, bool> serializable)
        {
            _serializable = serializable;
        }

        protected override IImmutableSet<MemberInfo> Callback(Type key) => GetWritableMembers(key).ToImmutableHashSet();

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

    class CanSerializeSpecification : ISpecification<TypeDefinition>
    {
        public static CanSerializeSpecification Default { get; } = new CanSerializeSpecification();
        CanSerializeSpecification() {}

        public bool IsSatisfiedBy(TypeDefinition definition)
            =>
            definition.IsPrimitive || 
            definition.IsDictionary || 
            definition.IsArray || 
            definition.IsEnumerable ||
            definition.IsObjectToSerialize;
    }

    public interface IMemberInstructionFactory
    {
        IInstruction Property(MemberInfo member);
        IInstruction Content(MemberInfo member);
    }

    class ObjectMemberInstructions : IInstructions
    {
        readonly private static Func<Type, IImmutableSet<MemberInfo>> Members = SerializableMembers.Default.Get;

        private readonly IInstructions _primary;
        private readonly Func<MemberInfo, bool> _specification;
        private readonly Func<Type, bool> _propertyTypeSpecification;
        private readonly Func<WriteContext, bool> _emit;
        private readonly Func<MemberInfo, bool> _deferred;
        private readonly Func<Type, IImmutableSet<MemberInfo>> _members;
        private readonly Func<MemberInfo, IInstruction> _property, _content;

        public ObjectMemberInstructions(IInstructions primary, IInstructionSpecification specification)
            : this(primary, specification, specification.Defer, new MemberInstructionFactory(primary)) {}

        public ObjectMemberInstructions(IInstructions primary, IInstructionSpecification specification, Func<MemberInfo, bool> deferred, IMemberInstructionFactory factory)
            : this(primary, specification.IsSatisfiedBy, specification.IsSatisfiedBy, specification.IsSatisfiedBy, deferred, factory.Property, factory.Content, Members) {}

        public ObjectMemberInstructions(
            IInstructions primary,
            Func<MemberInfo, bool> specification, 
            Func<Type, bool> propertyTypeSpecification, 
            Func<WriteContext, bool> emit,
            Func<MemberInfo, bool> deferred, 
            Func<MemberInfo, IInstruction> property,
            Func<MemberInfo, IInstruction> content, 
            Func<Type, IImmutableSet<MemberInfo>> members
        ) {
            _primary = primary;
            _specification = specification;
            _propertyTypeSpecification = propertyTypeSpecification;
            _emit = emit;
            _deferred = deferred;
            _property = property;
            _content = content;
            _members = members;
        }

        public IInstruction For(Type type)
        {
            var all = _members(type);
            var deferred = all.Where(_deferred).ToImmutableHashSet();
            var members = all.Except(deferred);
            var properties = members.Where(_specification).ToArray();
            var contents = members.Except(properties);
            
            var body = new CompositeInstruction(
                new CompositeInstruction(properties.Select(_property)),
                new EmitDeferredMembersInstruction(deferred, _property, _content, _emit, new EmitAttachedPropertiesInstruction(_primary, _propertyTypeSpecification)),
                new CompositeInstruction(contents.Select(_content))
            );
            var result = new StartNewMembersContextInstruction(all, body);
            return result;
        }
    }

    class EmitAttachedPropertiesInstruction : WriteInstructionBase
    {
        private readonly IInstructions _primary;
        private readonly Func<Type, bool> _specification;
        public EmitAttachedPropertiesInstruction(IInstructions primary, Func<Type, bool> specification)
        {
            _primary = primary;
            _specification = specification;
        }

        protected override void Execute(IWriteServices services)
        {
            var all = services.GetProperties();
            var properties = Properties(all).ToArray();
            foreach (var property in properties)
            {
                services.Property(property.Name, services.Serialize(property.Value));
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
                if (_specification(property.Value.GetType()))
                {
                    yield return property;
                }
            }
        }
    }

    class EmitDeferredMembersInstruction : DecoratedWriteInstruction
    {
        private readonly IImmutableSet<MemberInfo> _members;
        private readonly Func<MemberInfo, IInstruction> _property, _content;
        private readonly Func<WriteContext, bool> _specification;

        public EmitDeferredMembersInstruction(IImmutableSet<MemberInfo> members, Func<MemberInfo, IInstruction> property,
                                              Func<MemberInfo, IInstruction> content,
                                              Func<WriteContext, bool> specification, IInstruction instruction) : base(instruction)
        {
            _members = members;
            _property = property;
            _content = content;
            _specification = specification;
        }

        protected override void Execute(IWriteServices services)
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
        
        private readonly IInstructions _instructions;
        readonly private Func<MemberInfo, IInstruction> _property, _content;
        
        public MemberInstructionFactory(IInstructions primary)
        {
            _instructions = primary;
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
                        new FixedDecoratedInstruction(
                            _instructions, member.GetMemberType()
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

    class ObjectInstructions : ConditionalInstructionsBase
    {
        private readonly IInstructions _members;

        public ObjectInstructions(IInstructions primary, IInstructionSpecification specification) 
            : this(new ObjectMemberInstructions(primary, specification)) {}

        public ObjectInstructions(IInstructions members) : base(type => TypeDefinitionCache.GetDefinition(type).IsObjectToSerialize)
        {
            _members = members;
        }

        protected override IInstruction PerformBuild(Type type) => 
            new CompositeInstruction(new EmitTypeInstruction(type), _members.For(type));
    }

    class RootInstructions : IInstructions
    {
        private readonly IInstructions _builder;

        public RootInstructions(IInstructions builder)
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

    class CachedInstructions : IInstructions
    {
        private readonly ConditionalWeakTable<Type, IInstruction> _cache =
            new ConditionalWeakTable<Type, IInstruction>();

        private readonly ConditionalWeakTable<Type, IInstruction>.CreateValueCallback _callback;

        public CachedInstructions(IInstructions inner) : this(inner.For) {}

        public CachedInstructions(ConditionalWeakTable<Type, IInstruction>.CreateValueCallback callback)
        {
            _callback = callback;
        }

        public IInstruction For(Type type) => _cache.GetValue(type, _callback);
    }

    class EnumerableInstructions : ConditionalInstructionsBase
    {
        readonly private static Func<Type, bool> Specification = IsSatisfiedBy;

        private readonly IInstructions _builder;

        public EnumerableInstructions(IInstructions builder) : base(Specification)
        {
            _builder = builder;
        }

        private static bool IsSatisfiedBy(Type arg)
        {
            var definition = TypeDefinitionCache.GetDefinition(arg);
            var result = definition.IsArray || definition.IsEnumerable;
            return result;
        }

        protected override IInstruction PerformBuild(Type type)
        {
            var elementType = ElementTypeLocator.Default.Locate(type);
            var instructions = _builder.For(elementType);
            var result = new EmitEnumerableInstruction(instructions);
            return result;
        }
    }

    class FirstAssignedInstructions : IInstructions
    {
        readonly ICollection<IInstructions> _providers;

        public FirstAssignedInstructions(ICollection<IInstructions> providers)
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

    class FixedInstructions : IInstructions
    {
        private readonly IInstruction _instruction;
        public FixedInstructions(IInstruction instruction)
        {
            _instruction = instruction;
        }

        public IInstruction For(Type type) => _instruction;
    }

    class ContentInstructions : ConditionalInstructions
    {
        public static ContentInstructions Default { get; } = new ContentInstructions();
        ContentInstructions() : base(type => TypeDefinitionCache.GetDefinition(type).IsPrimitive, new FixedInstructions(StartNewContentContextInstruction.Default)) {}
    }

    class DictionaryInstructions : ConditionalInstructionsBase
    {
        private readonly IInstructions _builder;
        
        public DictionaryInstructions(IInstructions builder) : base(type => TypeDefinitionCache.GetDefinition(type).IsDictionary)
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
            var result = new EmitDictionaryInstruction(keys, values);
            return result;
        }
    }
}