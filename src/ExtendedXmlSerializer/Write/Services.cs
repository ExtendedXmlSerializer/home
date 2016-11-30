using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        readonly private static IInstructions BuildPlan = DefaultInstructionCompiler.Default.Compile();
        public static Serializer Default { get; } = new Serializer();
        Serializer() : this(BuildPlan) {}

        private readonly IInstructions _instructions;
        private readonly IObjectSerializer _serializer;
        private readonly IWriterExtension _extension;
        private readonly IWritingContext _context;
        private readonly IServiceProvider _services;

        public Serializer(IInstructions instructions)
            : this(instructions, DefaultWritingExtension.Default) {}

        public Serializer(params IWriterExtension[] extensions)
            : this(BuildPlan, extensions.Length == 1 ? extensions[0] : new CompositeExtension(extensions)) {}

        public Serializer(IInstructions instructions, IWriterExtension extension)
            : this(
                instructions, 
                ToolingObjectSerializer.Default, 
                new DefaultWritingContext(), 
                extension,
                DefaultServiceProvider.Default
        ) {}

        public Serializer(IInstructions instructions, IObjectSerializer serializer, IWritingContext context,
                          IWriterExtension extension, IServiceProvider services)
        {
            _instructions = instructions;
            _serializer = serializer;
            _extension = extension;
            _context = context;
            _services = services;
        }

        public void Serialize(IWriter writer, object instance)
        {
            var instruction = _instructions.For(instance.GetType());
            using (var services = new WritingServices(writer, _serializer, _context, _extension, _services))
            {
                using (services.Start(instance))
                {
                    instruction.Execute(services);
                }
            }
        }
    }

    public interface IWriter : IDisposable
    {
        void StartObject(string name);

        void EndObject();

        void Emit(string content);

        void Property(string name, string content);
    }

    public class Writer : IWriter
    {
        private readonly XmlWriter _writer;

        public Writer(XmlWriter writer)
        {
            _writer = writer;
        }

        public void StartObject(string name) => _writer.WriteStartElement(name);
        public void Emit(string content) => _writer.WriteString(content);
        public void EndObject() => _writer.WriteEndElement();
        
        public void Property(string name, string content) => _writer.WriteAttributeString(name, content);

        public void Dispose() => _writer.Dispose();
    }

    public interface IWritingServices : IWriter, IObjectSerializer, IWritingContext, IServiceProvider, IWriterExtension {}

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
        IEnumerable<WriteContext> History { get; }

        void Attach(IAttachedProperty property);
        IImmutableList<IAttachedProperty> GetProperties();

        IDisposable Start(object root);
        IDisposable New(object instance);
        IDisposable New(IImmutableList<MemberInfo> members);
        IDisposable New(MemberInfo member);
        IDisposable New(string content);
    }

    public class AttachedProperties : WeakCache<object, ICollection<IAttachedProperty>>
    {
        public AttachedProperties() : base(key => new Collection<IAttachedProperty>()) {}
    }

    class DefaultWritingContext : IWritingContext
    {
        readonly private AttachedProperties _properties = new AttachedProperties();
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

        public void Attach(IAttachedProperty property) => _properties.Get(Current.Instance).Add(property);
        public IImmutableList<IAttachedProperty> GetProperties()
        {
            var list = _properties.Get(Current.Instance);
            var result = list.ToImmutableList();
            list.Clear();
            return result;
        }

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

        public IEnumerable<WriteContext> History => _chain.Reverse().ToImmutableList();
    }

    class WritingServices : IWritingServices
    {
        private readonly IWriter _writer;
        private readonly IObjectSerializer _serializer;
        private readonly IWritingContext _context;
        private readonly IWriterExtension _extension;
        private readonly IServiceProvider _services;
        
        public WritingServices(IWriter writer, IObjectSerializer serializer, IWritingContext context, IWriterExtension extension, IServiceProvider services)
        {
            _writer = writer;
            _serializer = serializer;
            _context = context;
            _extension = extension;
            _services = services;
        }

        public object GetService(Type serviceType) => _services.GetService(serviceType);

        public void StartObject(string name) => _writer.StartObject(name);

        public void EndObject() => _writer.EndObject();

        public void Emit(string content) => _writer.Emit(content);

        public void Property(string name, string content) => _writer.Property(name, content);

        public string Serialize(object instance) => _serializer.Serialize(instance);

        public void Dispose()
        {
            Finished(this);
            _writer.Dispose();
            // throw new InvalidOperationException("This writer does not support this operation.");
        }

        public bool Starting(IWritingServices services) => _extension.Starting(services);
        public void Finished(IWritingServices services) => _extension.Finished(services);

        public IDisposable Start(object root)
        {
            var result = _context.Start(root);
            Starting(this);
            return result;
        }

        public void Attach(IAttachedProperty property) => _context.Attach(property);
        public IImmutableList<IAttachedProperty> GetProperties() => _context.GetProperties();

        public IDisposable New(object instance) => _context.New(instance);
        public IDisposable New(IImmutableList<MemberInfo> members) => _context.New(members);
        public IDisposable New(MemberInfo member) => _context.New(member);
        public IDisposable New(string content) => _context.New(content);
        public WriteContext Current => _context.Current;
        public IEnumerable<WriteContext> History => _context.History;
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

        public EncryptedObjectSerializer(ISerializationToolsFactory factory, IObjectSerializer inner, IWritingContext context)
        {
            _factory = factory;
            _inner = inner;
            _context = context;
        }

        public string Serialize(object instance)
        {
            var content = _inner.Serialize(instance);
            var algorithm = _factory?.EncryptionAlgorithm;
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