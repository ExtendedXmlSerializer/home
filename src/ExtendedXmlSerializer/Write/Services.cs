using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface ISerializer
    {
        void Serialize(Assignment assignment);
    }

    /*public interface ISerialization : IWritingContext, IWriter
    {
        object Instance { get; }
    }*/

    /*class Serialization : ISerialization
    {
        private readonly IWritingContext _context;
        private readonly IWriter _writer;

        public Serialization(IWritingContext context, IWriter writer, object instance)
        {
            Instance = instance;
            _context = context;
            _writer = writer;
        }

        public object Instance { get; }
        void IDisposable.Dispose() => _writer.Dispose();
        void IWriter.BeginContent(string name) => _writer.BeginContent(name);
        void IWriter.EndContent() => _writer.EndContent();
        void IWriter.Emit(object instance) => _writer.Emit(instance);
        void IWriter.Emit(IProperty property) => _writer.Emit(property);
        WriteContext IWritingContext.Current => _context.Current;
        IEnumerable<WriteContext> IWritingContext.Hierarchy => _context.Hierarchy;
        IDisposable IWritingContext.Start(object root) => _context.Start(root);
        IDisposable IWritingContext.New(object instance) => _context.New(instance);
        IDisposable IWritingContext.New(IImmutableList<MemberInfo> members) => _context.New(members);
        IDisposable IWritingContext.New(MemberInfo member) => _context.New(member);
        IDisposable IWritingContext.NewMemberContext() => _context.NewMemberContext();
    }*/

    public class Serializer : ISerializer
    {
        readonly private static IWritePlan Plan = DefaultWritePlanComposer.Default.Compose();
        
        private readonly IWritePlan _plan;
        private readonly IWritingFactory _factory;
        
        public Serializer(IWritingFactory factory) : this(Plan, factory) {}

        public Serializer(IWritePlan plan, IWritingFactory factory)
        {
            _plan = plan;
            _factory = factory;
        }

        public void Serialize(Assignment assignment)
        {
            using (var writing = _factory.Create(assignment))
            {
                using (writing.Start(assignment.Instance))
                {
                    var instruction = _plan.For(assignment.Instance.GetType());
                    instruction.Execute(writing);
                }
            }
        }
    }

    public class CompositeServiceProvider : IServiceProvider
    {
        private readonly IEnumerable<IServiceProvider> _providers;
        private readonly IEnumerable<object> _services;

        public CompositeServiceProvider(params object[] services) : this(services.ToImmutableHashSet()) {}

        CompositeServiceProvider(IImmutableSet<object> items) : this(items.OfType<IServiceProvider>().ToImmutableHashSet(), items) {}

        CompositeServiceProvider(IEnumerable<IServiceProvider> providers, IEnumerable<object> services)
        {
            _providers = providers;
            _services = services;
        }

        public object GetService(Type serviceType) => _services.FirstOrDefault(serviceType.GetTypeInfo().IsInstanceOfType) ?? FromServices(serviceType);

        private object FromServices(Type serviceType)
        {
            foreach (var service in _providers)
            {
                var result = service.GetService(serviceType);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }

    public interface IWriter : IDisposable
    {
        void BeginContent(string name);

        void EndContent();

        void Emit(object instance);

        void Emit(IProperty property);
    }

    public interface INamespaceLocator : IFactory<object, string>
    {}

    class NamespaceLocator : INamespaceLocator
    {
        public static NamespaceLocator Default { get; } = new NamespaceLocator();
        NamespaceLocator() {}

        public string Create(object parameter)
        {
            return null;
        }
    }

    class Writer : CompositeServiceProvider, IWriter
    {
        private readonly IObjectSerializer _serializer;
        private readonly INamespaceLocator _locator;
        private readonly XmlWriter _writer;

        public Writer(XmlWriter writer) : this(ObjectSerializer.Default, NamespaceLocator.Default, writer) {}

        public Writer(IObjectSerializer serializer, INamespaceLocator locator, XmlWriter writer) : base(serializer, writer)
        {
            _serializer = serializer;
            _locator = locator;
            _writer = writer;
        }

        public void BeginContent(string name) => _writer.WriteStartElement(name);
        public void EndContent() => _writer.WriteEndElement();
        public void Emit(object instance) => _writer.WriteString(_serializer.Serialize(instance));

        public void Emit(IProperty property) => _writer.WriteAttributeString(property.Name, _locator.Create(property), _serializer.Serialize(property.Value));

        public void Dispose() => _writer.Dispose();
    }

    public interface IWriting : IWriter, IWritingContext, IServiceProvider, IWritingExtension
    {
        void Attach(IProperty property);
        IImmutableList<IProperty> GetProperties();
    }

    public enum WriteState { Root, Instance, Members, Member, MemberValue }

    public class MemberContexts : WeakCacheBase<object, IImmutableList<MemberContext>>
    {
        public static MemberContexts Default { get; } = new MemberContexts();
        MemberContexts() {}

        protected override IImmutableList<MemberContext> Callback(object key) => Yield(key).ToImmutableList();

        public MemberContext Locate(object instance, MemberInfo member)
        {
            foreach (var memberContext in Get(instance))
            {
                if (MemberInfoEqualityComparer.Default.Equals(memberContext.Metadata, member))
                {
                    return memberContext;
                }
            }
            throw new InvalidOperationException($"Could not find the member '{member}' for instance of type '{instance.GetType()}'");
        }

        IEnumerable<MemberContext> Yield(object key)
        {
            var members = SerializableMembers.Default.Get(key.GetType());
            foreach (var member in members)
            {
                var getter = Getters.Default.Get(member);
                yield return new MemberContext(member, getter(key));
            }
        }
    }

    public struct MemberContext
    {
        public MemberContext(MemberInfo member, object value = null) : this(member, member.GetMemberType(), member.IsWritable(), value) {}

        public MemberContext(MemberInfo metadata, Type memberType, bool isWritable, object value)
        {
            Metadata = metadata;
            MemberType = memberType;
            IsWritable = isWritable;
            Value = value;
        }

        public MemberInfo Metadata { get; }
        public Type MemberType { get; }
        public bool IsWritable { get; }
        public object Value { get; }
    }

    public struct WriteContext
    {
        public WriteContext(WriteState state, object root, object instance, IImmutableList<MemberInfo> members,
                            MemberContext? member)
        {
            State = state;
            Root = root;
            Instance = instance;
            Members = members;
            Member = member;
        }

        public WriteState State { get; }
        public object Root { get; }
        public object Instance { get; }
        public IImmutableList<MemberInfo> Members { get; }
        public MemberContext? Member { get; }
    }

    public interface IProperty
    {
        string Name { get; }

        object Value { get; }
    }

    abstract class PropertyBase : IProperty
    {
        protected PropertyBase(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }
    }

    public interface IWritingContext
    {
        WriteContext Current { get; }
        IEnumerable<WriteContext> Hierarchy { get; }

        IDisposable Start(object root);
        IDisposable New(object instance);
        IDisposable New(IImmutableList<MemberInfo> members);
        IDisposable New(MemberInfo member);
        IDisposable NewMemberContext();
    }

    class DefaultWritingContext : IWritingContext
    {
        readonly private Stack<WriteContext> _chain = new Stack<WriteContext>();
        readonly private DelegatedDisposable _popper;

        public DefaultWritingContext()
        {
            _popper = new DelegatedDisposable(Undo);
        }

        public WriteContext Current => _chain.FirstOrDefault();

        IDisposable New(WriteContext context)
        {
            _chain.Push(context);
            return _popper;
        }
        void Undo() => _chain.Pop();

        public IDisposable Start(object root)
        {
            if (_chain.Any())
            {
                throw new InvalidOperationException("A request to start a new writing context was made, but it has already started.");
            }
            return New(new WriteContext(WriteState.Root, root, null, null, null));
        }

        public IDisposable New(object instance)
        {
            var previous = _chain.Peek();
            var result = New(new WriteContext(WriteState.Instance, previous.Root, instance, null, null));
            return result;
        }

        public IDisposable New(IImmutableList<MemberInfo> members)
        {
            var previous = _chain.Peek();
            var result = New(new WriteContext(WriteState.Members, previous.Root, previous.Instance, members, null));
            return result;
        }
        
        public IDisposable New(MemberInfo member)
        {
            var previous = _chain.Peek();
            var found = MemberContexts.Default.Locate(previous.Instance, member);
            var context = new WriteContext(WriteState.Member, previous.Root, previous.Instance, previous.Members,
                                           found);
            var result = New(context);
            return result;
        }

        public IDisposable NewMemberContext()
        {
            var previous = _chain.Peek();
            var context = new WriteContext(WriteState.MemberValue, previous.Root, previous.Instance, previous.Members, previous.Member);
            var result = New(context);
            return result;
        }
        
        public IEnumerable<WriteContext> Hierarchy
        {
            get
            {
                foreach (var context in _chain)
                {
                    yield return context;
                }
            }
        }
    }

    public interface IAttachedProperties
    {
        void Attach(object instance, IProperty property);
        ICollection<IProperty> GetProperties(object instance);
    }

    class AttachedProperties : IAttachedProperties
    {
        public static AttachedProperties Default { get; } = new AttachedProperties();
        AttachedProperties() {}

        private readonly WeakCache<object, ICollection<IProperty>> 
            _properties = new WeakCache<object, ICollection<IProperty>>(_ => new Collection<IProperty>());

        public void Attach(object instance, IProperty property) => _properties.Get(instance).Add(property);
        public ICollection<IProperty> GetProperties(object instance) => _properties.Get(instance);
    }

    /*public interface IWriterFactory : IParameterizedSource<IWritingContext, IWriter> {}

    class WriterFactory : IWriterFactory
    {
        private readonly ISerializationToolsFactory _tools;

        public WriterFactory(ISerializationToolsFactory tools)
        {
            _tools = tools;
        }

        public IWriter Get(IWritingContext parameter)
        {
            
        }
    }*/

    public interface IWritingFactory : IFactory<Assignment, IWriting>, ISerializationToolsFactory, IServiceProvider
    {
        ICollection<IWritingExtension> Extensions { get; }
    }

    public struct Assignment : IDisposable
    {
        public Assignment(IWritingContext context, IWriter writer, object instance)
        {
            Context = context;
            Writer = writer;
            Instance = instance;
        }

        public IWritingContext Context { get; }
        public IWriter Writer { get; }
        public object Instance { get; }
        public void Dispose() => Writer.Dispose();
    }

    public interface IAssignmentFactory
    {
        Assignment Create(Stream stream, object instance);
    }

    public class AssignmentFactory : IAssignmentFactory
    {
        private readonly Func<IWritingContext> _context;
        private readonly ISerializationToolsFactory _tools;
        private readonly INamespaceLocator _locator;

        public AssignmentFactory(ISerializationToolsFactory tools)
            : this(tools, NamespaceLocator.Default, () => new DefaultWritingContext()) {}

        public AssignmentFactory(ISerializationToolsFactory tools, INamespaceLocator locator, Func<IWritingContext> context)
        {
            _tools = tools;
            _locator = locator;
            _context = context;
        }

        public Assignment Create(Stream stream, object instance)
        {
            var context = _context();
            var serializer = new EncryptedObjectSerializer(new EncryptionSpecification(_tools, context), _tools);
            var writer = new Writer(serializer, _locator, XmlWriter.Create(stream));
            var result = new Assignment(context, writer, instance);
            return result;
        }
    }

    public class WritingFactory : CompositeServiceProvider, IWritingFactory
    {
        private readonly ISerializationToolsFactory _factory;

        public WritingFactory(ISerializationToolsFactory factory, ICollection<object> services) : this(factory, services, new HashSet<IWritingExtension>()) {}

        public WritingFactory(ISerializationToolsFactory factory, ICollection<object> services, ICollection<IWritingExtension> extensions) : base(services)
        {
            _factory = factory;
            Extensions = extensions;
        }

        public ICollection<IWritingExtension> Extensions { get; }
        
        public IWriting Create(Assignment parameter)
        {
            var serializer = new EncryptedObjectSerializer(new EncryptionSpecification(this, parameter.Context), this);
            var extension = new CompositeWritingExtension(Extensions);
            var result = new Writing(parameter.Writer, parameter.Context, extension, serializer,
                                     /*services:*/parameter.Writer, parameter.Context, parameter.Instance, extension, this);
            return result;
        }

        IExtendedXmlSerializerConfig ISerializationToolsFactory.GetConfiguration(Type type) => _factory.GetConfiguration(type);

        IPropertyEncryption ISerializationToolsFactory.EncryptionAlgorithm => _factory.EncryptionAlgorithm;
    }

    class Writing : IWriting
    {
        private readonly IWriter _writer;
        private readonly IAttachedProperties _properties;
        private readonly IWritingContext _context;
        private readonly IWritingExtension _extension;
        private readonly IServiceProvider _services;

        public Writing(IWriter writer, IWritingContext context, IWritingExtension extension, params object[] services)
            : this(writer, context, extension, AttachedProperties.Default, new CompositeServiceProvider(services)) {}

        public Writing(IWriter writer, IWritingContext context, IWritingExtension extension,
                       IAttachedProperties properties, IServiceProvider services)
        {
            _writer = writer;
            _context = context;
            _extension = extension;
            _properties = properties;
            _services = services;
        }

        public object GetService(Type serviceType) => serviceType.GetTypeInfo().IsInstanceOfType(this) ? this : _services.GetService(serviceType);

        public void BeginContent(string name) => _writer.BeginContent(name);

        public void EndContent() => _writer.EndContent();
        public void Emit(object instance) => _writer.Emit(instance);
        public void Emit(IProperty property) => _writer.Emit(property);

        public void Dispose()
        {
            Finished(this);
            _writer.Dispose();
        }

        public bool Starting(IWriting writing) => _extension.Starting(writing);
        public void Finished(IWriting services) => _extension.Finished(services);

        public IDisposable Start(object root)
        {
            var result = _context.Start(root);
            Starting(this);
            return result;
        }

        public void Attach(IProperty property) => _properties.Attach(_context.Current.Instance, property);
        public IImmutableList<IProperty> GetProperties()
        {
            var list = _properties.GetProperties(_context.Current.Instance);
            var result = list.ToImmutableList();
            list.Clear();
            return result;
        }

        public IDisposable New(object instance) => _context.New(instance);
        public IDisposable New(IImmutableList<MemberInfo> members) => _context.New(members);
        public IDisposable New(MemberInfo member) => _context.New(member);
        public IDisposable NewMemberContext() => _context.NewMemberContext();

        public IDisposable New(string text) => _context.New(text);
        public WriteContext Current => _context.Current;
        public IEnumerable<WriteContext> Hierarchy => _context.Hierarchy;
    }

    public interface IObjectSerializer
    {
        string Serialize(object instance);
    }

    class EncryptionSpecification : ISpecification<object>
    {
        private readonly ISerializationToolsFactory _factory;
        private readonly IWritingContext _context;
        public EncryptionSpecification(ISerializationToolsFactory factory, IWritingContext context)
        {
            _factory = factory;
            _context = context;
        }

        public bool IsSatisfiedBy(object parameter)
        {
            var context = _context.GetContextWithMember();
            if (context != null)
            {
                var configuration = _factory.GetConfiguration(context?.Instance.GetType());
                if (configuration != null)
                {
                    var member = context?.Member?.Metadata;
                    if (member != null)
                    {
                        var result =
                            configuration.CheckPropertyEncryption(member.Name);
                        return result;
                    }
                }
            }
            return false;
        }
    }

    class EncryptedObjectSerializer : IObjectSerializer
    {
        private readonly ISpecification<object> _specification;
        private readonly ISerializationToolsFactory _factory;
        private readonly IObjectSerializer _inner;
        

        public EncryptedObjectSerializer(ISpecification<object> specification, ISerializationToolsFactory factory)
            : this(specification, factory, ObjectSerializer.Default)
        {
            _specification = specification;
        }

        public EncryptedObjectSerializer(ISpecification<object> specification, ISerializationToolsFactory factory, IObjectSerializer inner)
        {
            _specification = specification;
            _factory = factory;
            _inner = inner;
        }

        public string Serialize(object instance)
        {
            var text = _inner.Serialize(instance);
            var algorithm = _factory.EncryptionAlgorithm;
            if (algorithm != null && _specification.IsSatisfiedBy(instance))
            {
                var result = algorithm.Encrypt(text);
                return result;
            }
            return text;
        }
    }
}