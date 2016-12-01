using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWriterExtension
    {
        bool Starting(IWritingServices services);
        void Finished(IWritingServices services);
    }

    public class DefaultWriteExtensions : CompositeExtension
    {
        public DefaultWriteExtensions(ISerializationToolsFactory factory) : base(
            new ObjectReferencesExtension(factory),
            new VersionExtension(factory),
            new CustomSerializationExtension(factory)
        ) {}
    }

    public class CompositeExtension : IWriterExtension
    {
        private readonly IEnumerable<IWriterExtension> _extensions;

        public CompositeExtension(params IWriterExtension[] extensions) : this(extensions.ToImmutableList()) {}

        public CompositeExtension(IEnumerable<IWriterExtension> extensions)
        {
            _extensions = extensions;
        }

        public bool Starting(IWritingServices services)
        {
            foreach (var extension in _extensions)
            {
                if (!extension.Starting(services))
                {
                    return false;
                }
            }
            return true;
        }

        public void Finished(IWritingServices services)
        {
            foreach (var extension in _extensions)
            {
                extension.Finished(services);
            }
        }
    }

    /*class ExtensionInstruction : DecoratedWriteInstruction
    {
        protected ExtensionInstruction(IInstruction instruction) : base(instruction) {}

        protected override void Execute(IWriteServices services)
        {
            if (services.Starting(services))
            {
                base.Execute(services);
            }
            services.Finished(services);
        }
    }*/

    class ToolingObjectSerializer : IObjectSerializer
    {
        public static ToolingObjectSerializer Default { get; } = new ToolingObjectSerializer();
        ToolingObjectSerializer() {}

        public string Serialize(object instance) => instance as string ?? PrimitiveValueTools.SetPrimitiveValue(instance);
    }

    public class DefaultWritingExtension : WritingExtensionBase
    {
        public static DefaultWritingExtension Default { get; } = new DefaultWritingExtension();
        DefaultWritingExtension() {}
    }

    public abstract class WritingExtensionBase : IWriterExtension
    {
        public virtual bool Starting(IWritingServices services)
        {
            var current = services.Current;
            if (current.Content != null)
            {
                return StartingContent(services, current.Instance, current.Member, current.Content);
            }

            if (current.Member != null)
            {
                return StartingMember(services, current.Instance, current.Member);
            }

            if (current.Members != null)
            {
                return StartingMembers(services, current.Instance, current.Members);
            }

            var result = current.Instance != null
                ? StartingInstance(services, current.Instance)
                : Initializing(services);
            return result;
        }

        protected virtual bool Initializing(IWritingServices services) => true;
        protected virtual bool StartingInstance(IWritingServices services, object instance) => true;
        protected virtual bool StartingMembers(IWritingServices services, object instance, IImmutableList<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWritingServices services, object instance, MemberInfo member) => true;
        protected virtual bool StartingContent(IWritingServices services, object instance, MemberInfo member, string content) => true;

        public virtual void Finished(IWritingServices services)
        {
            var current = services.Current;
            if (current.Content != null)
            {
                FinishedContent(services, current.Instance, current.Member, current.Content);
            }
            else if (current.Member != null)
            {
                FinishedMember(services, current.Instance, current.Member);
            }
            else if (current.Members != null)
            {
                FinishedMembers(services, current.Instance, current.Members);
            }
            else if (current.Instance != null)
            {
                FinishedInstance(services, current.Instance);
            }
            else
            {
                Completed(services);
            }
        }

        protected virtual void FinishedInstance(IWritingServices services, object instance) {}
        protected virtual void FinishedMembers(IWritingServices services, object instance, IImmutableList<MemberInfo> members) {}
        protected virtual void FinishedMember(IWritingServices services, object instance, MemberInfo member) {}
        protected virtual void FinishedContent(IWritingServices services, object instance, MemberInfo member, string content) {}
        protected virtual void Completed(IWritingServices services) {}
    }

    public abstract class ConfigurationWritingExtensionBase : WritingExtensionBase
    {
        private readonly ISerializationToolsFactory _factory;

        protected ConfigurationWritingExtensionBase(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        public override bool Starting(IWritingServices services)
        {
            var current = services.Current;
            if (current.Instance != null)
            {
                var type = current.Instance.GetType();
                var configuration = _factory.GetConfiguration(type);
                if (configuration != null)
                {
                    if (current.Content != null)
                    {
                        return StartingContent(services, configuration, current.Instance, current.Member, current.Content);
                    }

                    if (current.Member != null)
                    {
                        return StartingMember(services, configuration, current.Instance, current.Member);
                    }

                    var result = current.Members != null
                        ? StartingMembers(services, configuration, current.Instance, current.Members)
                        : StartingInstance(services, configuration, current.Instance);
                    return result;
                }
            }
            
            return base.Starting(services);
        }

        protected virtual bool StartingInstance(IWritingServices services, IExtendedXmlSerializerConfig configuration,
                                                object instance) => true;
        protected virtual bool StartingMembers(IWritingServices services, IExtendedXmlSerializerConfig configuration,
                                               object instance, IImmutableList<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWritingServices services, IExtendedXmlSerializerConfig configuration,
                                              object instance, MemberInfo member) => true;
        protected virtual bool StartingContent(IWritingServices services, IExtendedXmlSerializerConfig configuration,
                                               object instance, MemberInfo member, string content) => true;
    }

    public class CustomSerializationExtension : ConfigurationWritingExtensionBase
    {
        public CustomSerializationExtension(ISerializationToolsFactory factory) : base(factory) {}

        protected override bool StartingInstance(IWritingServices services, IExtendedXmlSerializerConfig configuration, object instance)
        {
            if (configuration.IsCustomSerializer)
            {
                configuration.WriteObject(services.Get<XmlWriter>(), instance);
                return false;
            }
            return base.StartingInstance(services, configuration, instance);
        }
    }

    public class VersionExtension : ConfigurationWritingExtensionBase
    {
        public VersionExtension(ISerializationToolsFactory factory) : base(factory) {}

        protected override bool StartingMembers(IWritingServices services, IExtendedXmlSerializerConfig configuration, object instance,
                                                IImmutableList<MemberInfo> members)
        {
            var version = configuration.Version;
            if (version > 0)
            {
                services.Attach(new VersionProperty(version));
            }
            return base.StartingMembers(services, configuration, instance, members);
        }
    }

    class VersionProperty : AttachedPropertyBase
    {
        public VersionProperty(int version) : base(ExtendedXmlSerializer.Version, version) {}
    }

    class ObjectReferenceProperty : AttachedPropertyBase
    {
        public ObjectReferenceProperty(string value) : base(ExtendedXmlSerializer.Ref, value) {}
    }

    class ObjectIdProperty : AttachedPropertyBase
    {
        public ObjectIdProperty(string value) : base(ExtendedXmlSerializer.Id, value) {}
    }

    public class ObjectReferencesExtension : ConfigurationWritingExtensionBase
    {
        private readonly IDictionary<object, object>
            _references = new ConcurrentDictionary<object, object>();

        public ObjectReferencesExtension(ISerializationToolsFactory factory) : base(factory) {}

        protected override bool StartingMembers(IWritingServices services, IExtendedXmlSerializerConfig configuration, object instance,
                                                IImmutableList<MemberInfo> members)
        {
            if (configuration.IsObjectReference)
            {
                var objectId = configuration.GetObjectId(instance);
                var contains = _references.ContainsKey(instance);
                var property = contains ? (IAttachedProperty)new ObjectReferenceProperty(objectId) : new ObjectIdProperty(objectId);
                var result = !contains;
                services.Property(property);
                if (result)
                {
                    _references.Add(instance, instance);
                }
                return result;
            }
            return base.StartingMembers(services, configuration, instance, members);
        }

        protected override bool Initializing(IWritingServices services)
        {
            _references.Clear();
            return base.Initializing(services);
        }

        protected override void Completed(IWritingServices services) => _references.Clear();
    }
}