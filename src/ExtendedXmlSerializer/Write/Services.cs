using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface ISerializer
    {
        void Serialize(IWriter writer, object instance);
    }

    class Serializer : ISerializer
    {
        readonly private static IWritePlan Plan = DefaultWritePlanComposer.Default.Compose();
        
        private readonly IWritePlan _plan;
        private readonly Func<IWriter, IWriting> _writing;
        
        public Serializer(Func<IWriter, IWriting> writing) : this(Plan, writing) {}

        public Serializer(IWritePlan plan, Func<IWriter, IWriting> writing)
        {
            _plan = plan;
            _writing = writing;
        }

        public void Serialize(IWriter writer, object instance)
        {
            using (var writing = _writing(writer))
            {
                using (writing.Start(instance))
                {
                    var instruction = _plan.For(instance.GetType());
                    instruction.Execute(writing);
                }
            }
        }
    }

    class CompositeServiceProvider : IServiceProvider
    {
        private readonly IEnumerable<IServiceProvider> _services;
        private readonly IEnumerable<object> _items;

        public CompositeServiceProvider(params object[] services) : this(services.ToImmutableHashSet()) {}

        CompositeServiceProvider(IImmutableSet<object> items) : this(items.OfType<IServiceProvider>().ToImmutableHashSet(), items) {}

        CompositeServiceProvider(IEnumerable<IServiceProvider> services, IEnumerable<object> items)
        {
            _services = services;
            _items = items;
        }

        public object GetService(Type serviceType) => _items.FirstOrDefault(serviceType.GetTypeInfo().IsInstanceOfType) ?? FromServices(serviceType);

        private object FromServices(Type serviceType)
        {
            foreach (var service in _services)
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
        void StartObject(string name);

        void EndObject();

        void Emit(string content);

        void Property(string name, string content);
    }

    class Writer : CompositeServiceProvider, IWriter
    {
        private readonly XmlWriter _writer;

        public Writer(XmlWriter writer) : base(writer)
        {
            _writer = writer;
        }

        public void StartObject(string name) => _writer.WriteStartElement(name);
        public void Emit(string content) => _writer.WriteString(content);
        public void EndObject() => _writer.WriteEndElement();
        
        public void Property(string name, string content) => _writer.WriteAttributeString(name, content);

        public void Dispose() => _writer.Dispose();
    }

    public interface IWriting : IWriter, IObjectSerializer, IWritingContext, IServiceProvider, IWritingExtension
    {
        void Attach(IAttachedProperty property);
        IImmutableList<IAttachedProperty> GetProperties();
    }

    public enum WriteState { Root, Instance, Members, Member, MemberValue, Content }

    public struct WriteContext
    {
        public WriteContext(WriteState state, object root, object instance, IImmutableList<MemberInfo> members,
                            MemberInfo member, Type memberType, object memberValue, string value)
        {
            State = state;
            Root = root;
            Instance = instance;
            Members = members;
            Member = member;
            MemberType = memberType;
            MemberValue = memberValue;
            Value = value;
        }

        public WriteState State { get; set; }
        public object Root { get; }
        public object Instance { get; }
        public IImmutableList<MemberInfo> Members { get; }
        public MemberInfo Member { get; }
        public Type MemberType { get; }
        public object MemberValue { get; }
        public string Value { get; }
    }

    public interface IAttachedProperty
    {
        string Name { get; }

        object Value { get; }
    }

    abstract class AttachedPropertyBase : IAttachedProperty
    {
        protected AttachedPropertyBase(string name, object value)
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
        IDisposable NewMemberValue();
        IDisposable New(string value);
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
            return New(new WriteContext(WriteState.Root, root, null, null, null, null, null, null));
        }

        public IDisposable New(object instance)
        {
            var previous = _chain.Peek();
            var result = New(new WriteContext(WriteState.Instance, previous.Root, instance, null, null, null, null, null));
            return result;
        }

        public IDisposable New(IImmutableList<MemberInfo> members)
        {
            var previous = _chain.Peek();
            var result = New(new WriteContext(WriteState.Members, previous.Root, previous.Instance, members, null, null, null, null));
            return result;
        }
        
        public IDisposable New(MemberInfo member)
        {
            var previous = _chain.Peek();
            var value = Getters.Default.Get(member).Invoke(previous.Instance);
            var context = new WriteContext(WriteState.Member, previous.Root, previous.Instance, previous.Members, member, member.GetMemberType(), value, null);
            var result = New(context);
            return result;
        }

        public IDisposable NewMemberValue()
        {
            var previous = _chain.Peek();
            var context = new WriteContext(WriteState.MemberValue, previous.Root, previous.Instance, previous.Members, previous.Member, previous.MemberType, previous.MemberValue, null);
            var result = New(context);
            return result;
        }
        
        public IDisposable New(string value)
        {
            var previous = _chain.Peek();
            var context = new WriteContext(WriteState.Content, previous.Root, previous.Instance, previous.Members, previous.Member, previous.MemberType, previous.MemberValue, value);
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
        void Attach(object instance, IAttachedProperty property);
        ICollection<IAttachedProperty> GetProperties(object instance);
    }

    class AttachedProperties : IAttachedProperties
    {
        public static AttachedProperties Default { get; } = new AttachedProperties();
        AttachedProperties() {}

        private readonly WeakCache<object, ICollection<IAttachedProperty>> 
            _properties = new WeakCache<object, ICollection<IAttachedProperty>>(_ => new Collection<IAttachedProperty>());

        public void Attach(object instance, IAttachedProperty property) => _properties.Get(instance).Add(property);
        public ICollection<IAttachedProperty> GetProperties(object instance) => _properties.Get(instance);
    }

    class Writing : IWriting
    {
        private readonly IWriter _writer;
        private readonly IObjectSerializer _serializer;
        private readonly IAttachedProperties _properties;
        private readonly IWritingContext _context;
        private readonly IWritingExtension _extension;
        private readonly IServiceProvider _services;

        public Writing(IWriter writer, IWritingContext context, IWritingExtension extension, IObjectSerializer serializer, params object[] services)
            : this(writer, context, extension, serializer, AttachedProperties.Default, new CompositeServiceProvider(services)) {}

        public Writing(IWriter writer, IWritingContext context, IWritingExtension extension,
                       IObjectSerializer serializer, IAttachedProperties properties, IServiceProvider services)
        {
            _writer = writer;
            _context = context;
            _extension = extension;
            _serializer = serializer;
            _properties = properties;
            _services = services;
        }

        public object GetService(Type serviceType) => serviceType.GetTypeInfo().IsInstanceOfType(this) ? this : _services.GetService(serviceType);

        public void StartObject(string name) => _writer.StartObject(name);

        public void EndObject() => _writer.EndObject();

        public void Emit(string content) => _writer.Emit(content);

        public void Property(string name, string content) => _writer.Property(name, content);

        public string Serialize(object instance) => _serializer.Serialize(instance);

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

        public void Attach(IAttachedProperty property) => _properties.Attach(_context.Current.Instance, property);
        public IImmutableList<IAttachedProperty> GetProperties()
        {
            var list = _properties.GetProperties(_context.Current.Instance);
            var result = list.ToImmutableList();
            list.Clear();
            return result;
        }

        public IDisposable New(object instance) => _context.New(instance);
        public IDisposable New(IImmutableList<MemberInfo> members) => _context.New(members);
        public IDisposable New(MemberInfo member) => _context.New(member);
        public IDisposable NewMemberValue() => _context.NewMemberValue();

        public IDisposable New(string value) => _context.New(value);
        public WriteContext Current => _context.Current;
        public IEnumerable<WriteContext> Hierarchy => _context.Hierarchy;
    }

    public interface IObjectSerializer
    {
        string Serialize(object instance);
    }

    class EncryptedObjectSerializer : IObjectSerializer
    {
        private readonly ISerializationToolsFactory _factory;
        private readonly IObjectSerializer _inner;
        private readonly IWritingContext _context;

        public EncryptedObjectSerializer(ISerializationToolsFactory factory, IWritingContext context)
            : this(factory, ObjectSerializer.Default, context) {}

        public EncryptedObjectSerializer(ISerializationToolsFactory factory, IObjectSerializer inner, IWritingContext context)
        {
            _factory = factory;
            _inner = inner;
            _context = context;
        }

        public string Serialize(object instance)
        {
            var content = _inner.Serialize(instance);
            var algorithm = _factory.EncryptionAlgorithm;
            if (algorithm != null)
            {
                var context = _context.GetMemberContext();
                var encrypt = context?.Member == null ||
                              (_factory.GetConfiguration(context?.Instance.GetType())?.CheckPropertyEncryption(context?.Member.Name) ?? true);
                if (encrypt)
                {
                    var result = algorithm.Encrypt(content);
                    return result;
                }
            }

            
            return content;
        }
    }
}