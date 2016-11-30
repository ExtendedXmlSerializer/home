using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface ISerializer
    {
        void Serialize(IWriter writer, object instance);
    }

    public class Serializer : ISerializer
    {
        readonly private static IInstructionBuilder BuildPlan = DefaultInstructionBuildPlan.Default.Create();
        public static Serializer Default { get; } = new Serializer();
        Serializer() : this(BuildPlan) {}

        private readonly IInstructionBuilder _instructions;
        private readonly IObjectSerializer _serializer;
        private readonly IWriterExtension _extension;
        private readonly IWriteContext _context;
        private readonly IServiceProvider _services;

        public Serializer(IInstructionBuilder instructions)
            : this(instructions, DefaultWritingExtension.Default) {}

        public Serializer(params IWriterExtension[] extensions)
            : this(BuildPlan, extensions.Length == 1 ? extensions[0] : new CompositeExtension(extensions)) {}

        public Serializer(IInstructionBuilder instructions, IWriterExtension extension)
            : this(
                instructions, 
                ToolingObjectSerializer.Default, 
                new DefaultWriteContext(), 
                extension,
                DefaultServiceProvider.Default
        ) {}

        public Serializer(IInstructionBuilder instructions, IObjectSerializer serializer, IWriteContext context,
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
            var instruction = _instructions.Build(instance.GetType());
            using (var services = new WriteServices(writer, _serializer, _context, _extension, _services))
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

        void Member(string name, string content);
    }

    public class Writer : IWriter
    {
        private readonly XmlWriter _writer;

        public Writer(XmlWriter writer)
        {
            _writer = writer;
        }

        public void StartObject(string name) => _writer.WriteStartElement(name);

        public void EndObject() => _writer.WriteFullEndElement();
        public void Emit(string content) => _writer.WriteString(content);
        public void Member(string name, string content) => _writer.WriteAttributeString(name, content);

        public void Dispose() => _writer.Dispose();
    }

    public interface IWriteServices : IWriter, IObjectSerializer, IWriteContext, IServiceProvider, IWriterExtension {}

    public struct WriteContext
    {
        public WriteContext(object root, object instance, MemberInfo member, string content)
        {
            Root = root;
            Instance = instance;
            Member = member;
            Content = content;
        }

        public object Root { get; }
        public object Instance { get; }
        public MemberInfo Member { get; }
        public string Content { get; }
    }

    public interface IWriteContext
    {
        WriteContext Current { get; }
        IEnumerable<WriteContext> History { get; }

        IDisposable Start(object root);
        IDisposable New(object instance);
        IDisposable New(MemberInfo member);
        IDisposable New(string content);
    }

    class DefaultWriteContext : IWriteContext
    {
        readonly private Stack<WriteContext> _chain = new Stack<WriteContext>();
        readonly private DelegatedDisposable _popper;

        /*public DefaultWriteContext(object root) : this(new WriteContext(root, null, null, null)) {}

        public DefaultWriteContext(WriteContext context)*/
        public DefaultWriteContext()
        {
            _popper = new DelegatedDisposable(Undo); // Might need to address this.
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
            return New(new WriteContext(root, null, null, null));
        }

        public IDisposable New(object instance) => New(_chain.Last(), instance);
        IDisposable New(WriteContext root, object instance) => New(new WriteContext(root.Root, instance, root.Member, root.Content));

        public IDisposable New(MemberInfo member) => New(_chain.Last(), member);
        IDisposable New(WriteContext root, MemberInfo member) => New(new WriteContext(root.Root, root.Instance, member, root.Content));

        public IDisposable New(string content) => New(_chain.Last(), content);
        IDisposable New(WriteContext root, string content) => New(new WriteContext(root.Root, root.Instance, root.Member, content));

        public IEnumerable<WriteContext> History => _chain.Reverse().Immutable();
    }

    class WriteServices : IWriteServices
    {
        private readonly IWriter _writer;
        private readonly IObjectSerializer _serializer;
        private readonly IWriteContext _context;
        private readonly IWriterExtension _extension;
        private readonly IServiceProvider _services;
        
        public WriteServices(IWriter writer, IObjectSerializer serializer, IWriteContext context, IWriterExtension extension, IServiceProvider services)
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

        public void Member(string name, string content) => _writer.Member(name, content);

        public string Serialize(object instance) => _serializer.Serialize(instance);

        public void Dispose()
        {
            _extension.Finished(this);
            _writer.Dispose();
            // throw new InvalidOperationException("This writer does not support this operation.");
        }

        public bool Starting(IWriteServices services) => _extension.Starting(services);
        public void Finished(IWriteServices services) => _extension.Finished(services);

        public IDisposable Start(object root)
        {
            var result = _context.Start(root);
            _extension.Starting(this);
            return result;
        }

        public IDisposable New(object instance) => _context.New(instance);
        public IDisposable New(MemberInfo member) => _context.New(member);
        public IDisposable New(string content) => _context.New(content);
        public WriteContext Current => _context.Current;
        public IEnumerable<WriteContext> History => _context.History;
    }

    public interface IObjectSerializer
    {
        string Serialize(object instance);
    }

    public class EncryptedObjectSerializer : IObjectSerializer
    {
        private readonly ISerializationToolsFactory _factory;
        private readonly IObjectSerializer _inner;
        private readonly IWriteContext _context;

        public EncryptedObjectSerializer(ISerializationToolsFactory factory, IObjectSerializer inner, IWriteContext context)
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