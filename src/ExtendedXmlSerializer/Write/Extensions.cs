using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWriterExtension
    {
        bool Starting(IWriteServices services);
        void Finished(IWriteServices services);
    }

    public class DefaultWriteExtensions : CompositeExtension
    {
        public DefaultWriteExtensions(ISerializationToolsFactory factory) : base(
            new ObjectReferencesExtension(factory),
            new VersionExtension(factory)
        ) {}
    }

    public class CompositeExtension : IWriterExtension
    {
        private readonly IEnumerable<IWriterExtension> _extensions;

        public CompositeExtension(params IWriterExtension[] extensions) : this(extensions.Immutable()) {}

        public CompositeExtension(IEnumerable<IWriterExtension> extensions)
        {
            _extensions = extensions;
        }

        public bool Starting(IWriteServices services)
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

        public void Finished(IWriteServices services)
        {
            foreach (var extension in _extensions)
            {
                extension.Finished(services);
            }
        }
    }

    class ExtensionInstruction : DecoratedWriteInstruction
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
    }

    class ToolingObjectSerializer : IObjectSerializer
    {
        public static ToolingObjectSerializer Default { get; } = new ToolingObjectSerializer();
        ToolingObjectSerializer() {}

        public string Serialize(object instance) => PrimitiveValueTools.SetPrimitiveValue(instance);
    }

    public class DefaultWritingExtension : WritingExtensionBase
    {
        public static DefaultWritingExtension Default { get; } = new DefaultWritingExtension();
        DefaultWritingExtension() {}
    }

    public abstract class WritingExtensionBase : IWriterExtension
    {
        public virtual bool Starting(IWriteServices services)
        {
            if (services.Current.Content != null)
            {
                return StartingContent(services, services.Current.Content);
            }

            if (services.Current.Member != null)
            {
                return StartingMember(services, services.Current.Member);
            }

            var result = services.Current.Instance != null
                ? StartingInstance(services, services.Current.Instance)
                : Initializing(services);
            return result;
        }

        protected virtual bool Initializing(IWriteServices services) => true;
        protected virtual bool StartingInstance(IWriteServices services, object instance) => true;
        protected virtual bool StartingMember(IWriteServices services, MemberInfo member) => true;
        protected virtual bool StartingContent(IWriteServices services, string content) => true;

        public virtual void Finished(IWriteServices services)
        {
            if (services.Current.Content != null)
            {
                FinishedContent(services, services.Current.Content);
            }
            else if (services.Current.Member != null)
            {
                FinishedMember(services, services.Current.Member);
            }
            else if (services.Current.Instance != null)
            {
                FinishedInstance(services, services.Current.Instance);
            }
            else
            {
                Completed(services);
            }
        }

        protected virtual void FinishedInstance(IWriteServices services, object instance) {}
        protected virtual void FinishedMember(IWriteServices services, MemberInfo member) {}
        protected virtual void FinishedContent(IWriteServices services, string content) {}
        protected virtual void Completed(IWriteServices services) {}
    }

    public class VersionExtension : WritingExtensionBase
    {
        private readonly ISerializationToolsFactory _factory;

        public VersionExtension(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        protected override bool StartingInstance(IWriteServices services, object instance)
        {
            var type = instance.GetType();
            var version = _factory
                .GetConfiguration(type)?
                .Version;

            if (version != null)
            {
                services.Member(ExtendedXmlSerializer.Version,
                                version.Value.ToString(CultureInfo.InvariantCulture));
            }
            return base.StartingInstance(services, instance);
        }
    }

    public class ObjectReferencesExtension : WritingExtensionBase
    {
        private readonly IDictionary<string, object>
            _references = new ConcurrentDictionary<string, object>();

        private readonly ISerializationToolsFactory _factory;

        public ObjectReferencesExtension(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        protected override bool StartingInstance(IWriteServices services, object instance)
        {
            var type = instance.GetType();
            var configuration = _factory.GetConfiguration(type);

            if (configuration?.IsObjectReference ?? false)
            {
                var objectId = configuration.GetObjectId(instance);
                var key = $"{type.FullName}{ExtendedXmlSerializer.Underscore}{objectId}";
                var result = _references.ContainsKey(key);
                var name = result ? ExtendedXmlSerializer.Ref : ExtendedXmlSerializer.Id;
                services.Member(name, objectId);
                if (!result)
                {
                    _references.Add(key, instance);
                }
                return result;
            }
            return base.StartingInstance(services, instance);
        }

        protected override bool Initializing(IWriteServices services)
        {
            _references.Clear();
            return base.Initializing(services);
        }

        protected override void Completed(IWriteServices services)
        {
            _references.Clear();
            base.Completed(services);
        }
    }
}