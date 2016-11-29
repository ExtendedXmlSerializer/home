using System;
using System.Reflection;
using System.Xml;

namespace ExtendedXmlSerialization.Write
{
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

    public interface IWriteServices : IWriter, IObjectSerializer, IRootContainer, IWriterExtension, IServiceProvider {}

    public interface IRootContainer
    {
        object Root { get; }
    }

    class WriteServices : IWriteServices
    {
        private readonly IWriter _writer;
        private readonly IObjectSerializer _serializer;
        private readonly IWriterExtension _extension;
        private readonly IServiceProvider _services;
        
        public WriteServices(IWriter writer, IObjectSerializer serializer, IWriterExtension extension, IServiceProvider services, object root)
        {
            _writer = writer;
            _serializer = serializer;
            _extension = extension;
            _services = services;
            Root = root;
        }

        public object Root { get; }

        public void Started(IWriteServices services) => _extension.Started(services);
        public void Finished(IWriteServices services) => _extension.Finished(services);
        public bool Started(IWriteServices services, object instance) => _extension.Started(services, instance);
        public void Finished(IWriteServices services, object instance) => _extension.Finished(services, instance);
        public bool Started(IWriteServices services, object instance, string content) => _extension.Started(services, instance, content);
        public void Finished(IWriteServices services, object instance, string content) => _extension.Finished(services, instance, content);
        public bool Started(IWriteServices services, object instance, MemberInfo member) => _extension.Started(services, instance, member);
        public void Finished(IWriteServices services, object instance, MemberInfo member) => _extension.Finished(services, instance, member);
        public bool Started(IWriteServices services, object instance, MemberInfo member, string content) => _extension.Started(services, instance, member, content);
        public void Finished(IWriteServices services, object instance, MemberInfo member, string content) => _extension.Finished(services, instance, member, content);

        public void Dispose()
        {
            throw new InvalidOperationException("This writer does not support this operation.");
        }

        public void StartObject(string name) => _writer.StartObject(name);

        public void EndObject() => _writer.EndObject();

        public void Emit(string content) => _writer.Emit(content);

        public void Member(string name, string content) => _writer.Member(name, content);

        public object GetService(Type serviceType) => _services.GetService(serviceType);

        public string Serialize(object instance) => _serializer.Serialize(instance);
    }

    public interface IObjectSerializer
    {
        string Serialize(object instance);
    }

    public class EncryptedContentProvider : IObjectSerializer
    {
        private readonly IExtendedXmlSerializer _serializer;
        private readonly IObjectSerializer _inner;
        private readonly string _memberName;

        public EncryptedContentProvider(IExtendedXmlSerializer serializer, IObjectSerializer inner, string memberName)
        {
            _serializer = serializer;
            _inner = inner;
            _memberName = memberName;
        }

        public string Serialize(object instance)
        {
            var content = _inner.Serialize(instance);
            var type = instance.GetType();
            var configuration = _serializer.SerializationToolsFactory?.GetConfiguration(type);

            if (configuration?.CheckPropertyEncryption(_memberName) ?? false)
            {
                var algorithm = _serializer.SerializationToolsFactory?.EncryptionAlgorithm;
                if (algorithm != null)
                {
                    var result = algorithm.Encrypt(content);
                    return result;
                }
            }
            return content;
        }
    }
}