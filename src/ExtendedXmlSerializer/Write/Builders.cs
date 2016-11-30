using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IInstructionBuildPlan
    {
        IInstructionBuilder Create();
    }

    class MemberInfoNameProvider : FixedNameProvider
    {
        public MemberInfoNameProvider(MemberInfo member) : base(member.GetCustomAttribute<XmlAttributeAttribute>()?.AttributeName ?? member.Name) {}
    }

    class IsPropertySpecification : ISpecification<MemberInfo>
    {
        public static IsPropertySpecification Default { get; } = new IsPropertySpecification();
        IsPropertySpecification() {}

        public bool IsSatisfiedBy(MemberInfo member) => member.IsDefined(typeof(XmlAttributeAttribute));
    }

    class AutoAttributeSpecification : ISpecification<MemberInfo>
    {
        public static AutoAttributeSpecification Default { get; } = new AutoAttributeSpecification();
        AutoAttributeSpecification() : this(IsPropertySpecification.Default.IsSatisfiedBy) {}

        private readonly Func<MemberInfo, bool> _contentSpecification;

        public AutoAttributeSpecification(Func<MemberInfo, bool> contentSpecification)
        {
            _contentSpecification = contentSpecification;
        }

        public bool IsSatisfiedBy(MemberInfo parameter)
            =>
            _contentSpecification(parameter) || TypeDefinitionCache.GetDefinition(parameter.GetMemberType()).IsPrimitive;
    }

    class AutoAttributeValueSpecification : ISpecification<WriteContext>
    {
        readonly private static Func<MemberInfo, bool> Specification = AutoAttributeSpecification.Default.IsSatisfiedBy;
        readonly private static Type Type = typeof(string);

        public static AutoAttributeValueSpecification Default { get; } = new AutoAttributeValueSpecification();
        AutoAttributeValueSpecification() {}

        private readonly Func<MemberInfo, bool> _contentSpecification;
        private readonly int _maxLength;

        public AutoAttributeValueSpecification(int maxLength = 128) : this(Specification, maxLength) {}

        public AutoAttributeValueSpecification(Func<MemberInfo, bool> contentSpecification, int maxLength = 128)
        {
            _contentSpecification = contentSpecification;
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
            return _contentSpecification(parameter.Member);
        }
    }

    class AutoAttributeValueInstructionBuildPlan : DefaultInstructionBuildPlan
    {
        public new static AutoAttributeValueInstructionBuildPlan Default { get; } = new AutoAttributeValueInstructionBuildPlan();
        AutoAttributeValueInstructionBuildPlan()
            : base(
                new InstructionPlanBuilders(
                    new DynamicObjectInstructionPlanBuilder(AutoAttributeValueSpecification.Default.IsSatisfiedBy)
                )
            ) {}
    }

    class AutoAttributeInstructionBuildPlan : DefaultInstructionBuildPlan
    {
        public new static AutoAttributeInstructionBuildPlan Default { get; } = new AutoAttributeInstructionBuildPlan();
        AutoAttributeInstructionBuildPlan()
            : base(
                new InstructionPlanBuilders(
                    new StaticObjectInstructionPlanBuilder(AutoAttributeSpecification.Default.IsSatisfiedBy)
                )
            ) {}
    }

    class DynamicObjectInstructionPlanBuilder : IParameterizedSource<IInstructionBuilder, IInstructionBuilder>
    {
        private readonly Func<WriteContext, bool> _specification;

        public DynamicObjectInstructionPlanBuilder(Func<WriteContext, bool> specification)
        {
            _specification = specification;
        }

        public IInstructionBuilder Get(IInstructionBuilder parameter) => new ObjectInstructionBuilder(_specification, parameter);
    }

    class StaticObjectInstructionPlanBuilder : IParameterizedSource<IInstructionBuilder, IInstructionBuilder>
    {
        public static StaticObjectInstructionPlanBuilder Default { get; } = new StaticObjectInstructionPlanBuilder();
        StaticObjectInstructionPlanBuilder() : this(IsPropertySpecification.Default.IsSatisfiedBy) {}

        private readonly Func<MemberInfo, bool> _specification;


        public StaticObjectInstructionPlanBuilder(Func<MemberInfo, bool> specification)
        {
            _specification = specification;
        }

        public IInstructionBuilder Get(IInstructionBuilder parameter) => new ObjectInstructionBuilder(_specification, parameter);
    }

    class InstructionPlanBuilders : IParameterizedSource<IInstructionBuilder, IEnumerable<IInstructionBuilder>>
    {
        private readonly IEnumerable<IParameterizedSource<IInstructionBuilder, IInstructionBuilder>> _sources;
        public static InstructionPlanBuilders Default { get; } = new InstructionPlanBuilders();
        InstructionPlanBuilders() : this(StaticObjectInstructionPlanBuilder.Default) {}

        public InstructionPlanBuilders(params IParameterizedSource<IInstructionBuilder, IInstructionBuilder>[] sources) : this(sources.AsEnumerable()) {}
        public InstructionPlanBuilders(IEnumerable<IParameterizedSource<IInstructionBuilder, IInstructionBuilder>> sources)
        {
            _sources = sources;
        }

        public IEnumerable<IInstructionBuilder> Get(IInstructionBuilder parameter) =>
            new IInstructionBuilder[]
            {
                ContentInstruction.Default,
                new DictionaryInstructionBuilder(parameter),
                new EnumerableInstructionBuilder(parameter)
            }
            .Concat(Sources(parameter))
            .Immutable();

        private IEnumerable<IInstructionBuilder> Sources(IInstructionBuilder parameter)
        {
            foreach (var source in _sources)
            {
                yield return source.Get(parameter);
            }
        }
    }

    class DefaultInstructionBuildPlan : IInstructionBuildPlan
    {
        public static DefaultInstructionBuildPlan Default { get; } = new DefaultInstructionBuildPlan();
        DefaultInstructionBuildPlan() : this(InstructionPlanBuilders.Default) {}

        private readonly IParameterizedSource<IInstructionBuilder, IEnumerable<IInstructionBuilder>> _builders;
        
        public DefaultInstructionBuildPlan(IParameterizedSource<IInstructionBuilder, IEnumerable<IInstructionBuilder>> builders)
        {
            _builders = builders;
        }

        public IInstructionBuilder Create()
        {
            var collection = new List<IInstructionBuilder>();
            var selector = new CachedInstructionBuilder(new FirstAssignedInstructionBuilder(collection));
            collection.AddRange(
                _builders.Get(selector)
            );
            var result = new CachedInstructionBuilder(new RootInstructionBuilder(selector));
            return result;
        }
    }


    public interface IInstructionBuilder
    {
        IInstruction Build(Type type);
    }

    class ConditionalInstructionBuilder : ConditionalInstructionBuilderBase
    {
        private readonly IInstructionBuilder _builder;
        public ConditionalInstructionBuilder(Func<Type, bool> specification, IInstructionBuilder builder) : base(specification)
        {
            _builder = builder;
        }
        protected override IInstruction PerformBuild(Type type) => _builder.Build(type);
    }

    abstract class ConditionalInstructionBuilderBase : IInstructionBuilder
    {
        private readonly Func<Type, bool> _specification;

        protected ConditionalInstructionBuilderBase(Func<Type, bool> specification)
        {
            _specification = specification;
        }

        public IInstruction Build(Type type) => _specification(type) ? PerformBuild(type) : null;
        protected abstract IInstruction PerformBuild(Type type);
    }

    class SerializableMembers : WeakCacheBase<Type, IEnumerable<MemberInfo>>
    {
        public static SerializableMembers Default { get; } = new SerializableMembers();
        SerializableMembers() : this(CanSerializeSpecification.Default.IsSatisfiedBy) {}

        private readonly Func<TypeDefinition, bool> _serializable;

        public SerializableMembers(Func<TypeDefinition, bool> serializable)
        {
            _serializable = serializable;
        }

        protected override IEnumerable<MemberInfo> Callback(Type key) => GetWritableMembers(key).Immutable();

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

    class DynamicStaticMemberInstructionBuilder : MemberInstructionBuilderBase
    {
        private readonly Func<WriteContext, bool> _specification;

        public DynamicStaticMemberInstructionBuilder(Func<WriteContext, bool> specification, IInstructionBuilder selector) : base(selector)
        {
            _specification = specification;
        }

        protected override IInstruction Create(IEnumerable<MemberInfo> members, IMemberInstructionFactory factory)
            => new DynamicMemberInstruction(_specification, members, factory);
    }

    class DynamicMemberInstruction : WriteInstructionBase
    {
        private readonly Func<WriteContext, bool> _specification;
        private readonly IEnumerable<MemberInfo> _members;
        private readonly IMemberInstructionFactory _factory;
        
        public DynamicMemberInstruction(Func<WriteContext, bool> specification, IEnumerable<MemberInfo> members, IMemberInstructionFactory factory)
        {
            _specification = specification;
            _members = members;
            _factory = factory;
        }

        protected override void Execute(IWriteServices services)
        {
            var instructions = new MemberInstructions(_members,
                                                      new Specification(_specification, services.Current)
                                                          .IsSatisfiedBy, _factory);
            foreach (var instruction in instructions)
            {
                instruction.Execute(services);
            }
        }

        sealed class Specification : ISpecification<MemberInfo>
        {
            private readonly Func<WriteContext, bool> _specification;
            private readonly WriteContext _context;

            public Specification(Func<WriteContext, bool> specification, WriteContext context)
            {
                _specification = specification;
                _context = context;
            }

            public bool IsSatisfiedBy(MemberInfo parameter) => _specification(new WriteContext(_context.Root, _context.Instance, parameter, null));
        }
    }

    class StaticMemberInstructionBuilder : MemberInstructionBuilderBase
    {
        private readonly Func<MemberInfo, bool> _specification;

        public StaticMemberInstructionBuilder(Func<MemberInfo, bool> specification, IInstructionBuilder selector) : base(selector)
        {
            _specification = specification;
        }

        protected override IInstruction Create(IEnumerable<MemberInfo> members, IMemberInstructionFactory factory)
            => new CompositeInstruction(new MemberInstructions(members, _specification, factory));
    }

    abstract class MemberInstructionBuilderBase : IInstructionBuilder
    {
        readonly private static Func<Type, IEnumerable<MemberInfo>> Members = SerializableMembers.Default.Get;

        private readonly Func<Type, IEnumerable<MemberInfo>> _members;
        private readonly IMemberInstructionFactory _factory;

        public MemberInstructionBuilderBase(IInstructionBuilder selector)
            : this(Members, new MemberInstructionFactory(selector)) {}

        public MemberInstructionBuilderBase(Func<Type, IEnumerable<MemberInfo>> members, IMemberInstructionFactory factory)
        {
            _members = members;
            _factory = factory;
        }

        public IInstruction Build(Type type) => Create(_members(type), _factory);
        protected abstract IInstruction Create(IEnumerable<MemberInfo> members, IMemberInstructionFactory factory);
    }

    public interface IMemberInstructionFactory
    {
        IInstruction Property(MemberInfo member);
        IInstruction Content(MemberInfo member);
    }

    class MemberInstructionFactory : IMemberInstructionFactory
    {
        readonly private static Func<MemberInfo, IInstruction> PropertyDelegate = CreateProperty;
        
        private readonly IInstructionBuilder _selector;
        readonly private Func<MemberInfo, IInstruction> _getProperty, _getContent;

        public MemberInstructionFactory(IInstructionBuilder selector)
        {
            _selector = selector;
            _getProperty = new Wrap(PropertyDelegate).Get;
            _getContent = new Wrap(CreateContent).Get;
        }

        public IInstruction Property(MemberInfo member) => _getProperty(member);
        static IInstruction CreateProperty(MemberInfo member) => new EmitObjectPropertyInstruction(member);

        public IInstruction Content(MemberInfo member) => _getContent(member);
        IInstruction CreateContent(MemberInfo member) =>
            new EmitObjectContentInstruction(
                    member,
                    new DeferredInstruction(
                        new FixedDecoratedInstruction(
                            _selector, member.DeclaringType).Get
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

    class MemberInstructions :  MemberInstructionsBase
    {
        private readonly Func<MemberInfo, bool> _propertySpecification;

        public MemberInstructions(IEnumerable<MemberInfo> source, Func<MemberInfo, bool> propertySpecification, IMemberInstructionFactory factory) : base(source, factory)
        {
            _propertySpecification = propertySpecification;
        }

        protected override bool IsProperty(MemberInfo member) => _propertySpecification(member);
    }

    abstract class MemberInstructionsBase : IEnumerable<IInstruction>
    {
        private readonly IEnumerable<MemberInfo> _source;
        private readonly Func<MemberInfo, IInstruction> _property;
        private readonly Func<MemberInfo, IInstruction> _content;

        protected MemberInstructionsBase(IEnumerable<MemberInfo> source, IMemberInstructionFactory factory) : this(source, factory.Property, factory.Content) {}

        protected MemberInstructionsBase(IEnumerable<MemberInfo> source, Func<MemberInfo, IInstruction> property, Func<MemberInfo, IInstruction> content)
        {
            _source = source;
            _property = property;
            _content = content;
        }

        protected abstract bool IsProperty(MemberInfo member);

        public IEnumerator<IInstruction> GetEnumerator()
        {
            var properties = _source.Where(IsProperty).ToArray();
            var content = _source.Except(properties).ToArray();
            var result = properties.Select(_property).Concat(content.Select(_content)).Immutable().GetEnumerator();
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class ObjectInstructionBuilder : ConditionalInstructionBuilderBase
    {
        private readonly IInstructionBuilder _members;

        public ObjectInstructionBuilder(Func<WriteContext, bool> contentSpecification, IInstructionBuilder builder) 
            : this(new DynamicStaticMemberInstructionBuilder(contentSpecification, builder)) {}
        public ObjectInstructionBuilder(Func<MemberInfo, bool> contentSpecification, IInstructionBuilder builder) 
            : this(new StaticMemberInstructionBuilder(contentSpecification, builder)) {}

        public ObjectInstructionBuilder(IInstructionBuilder members) : base(type => TypeDefinitionCache.GetDefinition(type).IsObjectToSerialize)
        {
            _members = members;
        }

        protected override IInstruction PerformBuild(Type type) => 
            new CompositeInstruction(new EmitTypeInstruction(type), _members.Build(type));
    }

    class RootInstructionBuilder : IInstructionBuilder
    {
        private readonly IInstructionBuilder _builder;

        public RootInstructionBuilder(IInstructionBuilder builder)
        {
            _builder = builder;
        }

        public IInstruction Build(Type type)
        {
            var content = _builder.Build(type);
            if (content == null)
            {
                throw new InvalidOperationException($"Could not find instruction for type '{type}'");
            }
            var result = new StartNewContextFromRootInstruction(new EmitObjectInstruction(type, content));
            return result;
        }
    }

    class CachedInstructionBuilder : IInstructionBuilder
    {
        private readonly ConditionalWeakTable<Type, IInstruction> _cache =
            new ConditionalWeakTable<Type, IInstruction>();

        private readonly ConditionalWeakTable<Type, IInstruction>.CreateValueCallback _callback;

        public CachedInstructionBuilder(IInstructionBuilder inner) : this(inner.Build) {}

        public CachedInstructionBuilder(ConditionalWeakTable<Type, IInstruction>.CreateValueCallback callback)
        {
            _callback = callback;
        }

        public IInstruction Build(Type type) => _cache.GetValue(type, _callback);
    }

    class EnumerableInstructionBuilder : ConditionalInstructionBuilderBase
    {
        readonly private static Func<Type, bool> Specification = IsSatisfiedBy;

        private readonly IInstructionBuilder _builder;

        public EnumerableInstructionBuilder(IInstructionBuilder builder) : base(Specification)
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
            var instructions = _builder.Build(elementType);
            var result = new EmitEnumerableInstruction(instructions);
            return result;
        }
    }

    class FirstAssignedInstructionBuilder : IInstructionBuilder
    {
        readonly ICollection<IInstructionBuilder> _providers;

        public FirstAssignedInstructionBuilder(ICollection<IInstructionBuilder> providers)
        {
            _providers = providers;
        }

        public IInstruction Build(Type type)
        {
            foreach (var provider in _providers)
            {
                var instruction = provider.Build(type);
                if (instruction != null)
                {
                    return instruction;
                }
            }
            return null;
        }
    }

    class FixedInstructionBuilder : IInstructionBuilder
    {
        private readonly IInstruction _instruction;
        public FixedInstructionBuilder(IInstruction instruction)
        {
            _instruction = instruction;
        }

        public IInstruction Build(Type type) => _instruction;
    }

    class ContentInstruction : ConditionalInstructionBuilder
    {
        public static ContentInstruction Default { get; } = new ContentInstruction();
        ContentInstruction() : base(type => TypeDefinitionCache.GetDefinition(type).IsPrimitive, new FixedInstructionBuilder(EmitContentInstruction.Default)) {}
    }

    class DictionaryInstructionBuilder : ConditionalInstructionBuilderBase
    {
        private readonly IInstructionBuilder _builder;
        
        public DictionaryInstructionBuilder(IInstructionBuilder builder) : base(type => TypeDefinitionCache.GetDefinition(type).IsDictionary)
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
            var keys = new EmitObjectInstruction(ExtendedXmlSerializer.Key, _builder.Build(arguments[0]));
            var values = new EmitObjectInstruction(ExtendedXmlSerializer.Value, _builder.Build(arguments[1]));
            var result = new EmitDictionaryInstruction(keys, values);
            return result;
        }
    }
}