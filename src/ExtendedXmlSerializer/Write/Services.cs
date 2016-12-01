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

    public class Serializer : ISerializer
    {
        readonly private static IWritePlan Plan = DefaultWritingPlan.Default.Compile();
        
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
            var instruction = _plan.For(instance.GetType());

            using (var services = _writing(writer))
            {
                using (services.Start(instance))
                {
                    instruction.Execute(services);
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

    public struct WriteContext
    {
        public WriteContext(object root, object instance, IImmutableList<MemberInfo> members, MemberInfo member, string content)
        {
            Root = root;
            Instance = instance;
            Members = members;
            Member = member;
            Content = content;
        }

        public object Root { get; }
        public object Instance { get; }
        public IImmutableList<MemberInfo> Members { get; }
        public MemberInfo Member { get; }
        public string Content { get; }
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
        IDisposable New(string content);
    }

    class DefaultWritingContext : IWritingContext
    {
        readonly private Stack<WriteContext> _chain = new Stack<WriteContext>();
        readonly private DelegatedDisposable _popper;

        /*public DefaultWriteContext(object root) : this(new WriteContext(root, null, null, null)) {}

        public DefaultWriteContext(WriteContext context)*/
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
            return New(new WriteContext(root, null, null, null, null));
        }

        public IDisposable New(object instance) => New(_chain.Peek(), instance);
        IDisposable New(WriteContext root, object instance) => New(new WriteContext(root.Root, instance, null, null, null));

        public IDisposable New(IImmutableList<MemberInfo> members) => New(_chain.Peek(), members);
        IDisposable New(WriteContext root, IImmutableList<MemberInfo> members) => New(new WriteContext(root.Root, root.Instance, members, null, null));

        public IDisposable New(MemberInfo member) => New(_chain.Peek(), member);
        IDisposable New(WriteContext root, MemberInfo member) => New(new WriteContext(root.Root, root.Instance, root.Members, member, null));

        public IDisposable New(string content) => New(_chain.Peek(), content);
        IDisposable New(WriteContext root, string content) => New(new WriteContext(root.Root, root.Instance, root.Members, root.Member, content));

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

        public bool Starting(IWriting services) => _extension.Starting(services);
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
        public IDisposable New(string content) => _context.New(content);
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
                var type = instance.GetType();
                var member = _context.Current.Member;
                var encrypt = member == null ||
                              (_factory.GetConfiguration(type)?.CheckPropertyEncryption(member.Name) ?? true);
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