using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerialization.Write
{
    public interface IWriterExtension
    {
        bool Starting(IWriteContext context);
        void Finished(IWriteContext context);
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

        public CompositeExtension(params IWriterExtension[] extensions) : this(extensions.ToImmutableList()) {}

        public CompositeExtension(IEnumerable<IWriterExtension> extensions)
        {
            _extensions = extensions;
        }

        public bool Starting(IWriteContext context)
        {
            foreach (var extension in _extensions)
            {
                if (!extension.Starting(context))
                {
                    return false;
                }
            }
            return true;
        }

        public void Finished(IWriteContext context)
        {
            foreach (var extension in _extensions)
            {
                extension.Finished(context);
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
        public virtual bool Starting(IWriteContext context)
        {
            var current = context.Current;
            if (current.Content != null)
            {
                return StartingContent(context, current.Instance, current.Member, current.Content);
            }

            if (current.Member != null)
            {
                return StartingMember(context, current.Instance, current.Member);
            }

            if (current.Members != null)
            {
                return StartingMembers(context, current.Instance, current.Members);
            }

            var result = current.Instance != null
                ? StartingInstance(context, current.Instance)
                : Initializing(context);
            return result;
        }

        protected virtual bool Initializing(IWriteContext context) => true;
        protected virtual bool StartingInstance(IWriteContext context, object instance) => true;
        protected virtual bool StartingMembers(IWriteContext context, object instance, IImmutableSet<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWriteContext context, object instance, MemberInfo member) => true;
        protected virtual bool StartingContent(IWriteContext context, object instance, MemberInfo member, string content) => true;

        public virtual void Finished(IWriteContext context)
        {
            var current = context.Current;
            if (current.Content != null)
            {
                FinishedContent(context, current.Instance, current.Member, current.Content);
            }
            else if (current.Member != null)
            {
                FinishedMember(context, current.Instance, current.Member);
            }
            else if (current.Members != null)
            {
                FinishedMembers(context, current.Instance, current.Members);
            }
            else if (current.Instance != null)
            {
                FinishedInstance(context, current.Instance);
            }
            else
            {
                Completed(context);
            }
        }

        protected virtual void FinishedInstance(IWriteContext context, object instance) {}
        protected virtual void FinishedMembers(IWriteContext context, object instance, IImmutableSet<MemberInfo> members) {}
        protected virtual void FinishedMember(IWriteContext context, object instance, MemberInfo member) {}
        protected virtual void FinishedContent(IWriteContext context, object instance, MemberInfo member, string content) {}
        protected virtual void Completed(IWriteContext context) {}
    }

    public class VersionExtension : WritingExtensionBase
    {
        private readonly ISerializationToolsFactory _factory;

        public VersionExtension(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        protected override bool StartingMembers(IWriteContext context, object instance, IImmutableSet<MemberInfo> members)
        {
            var type = instance.GetType();
            var version = _factory
                .GetConfiguration(type)?
                .Version;

            if (version != null)
            {
                context.Attach(new VersionProperty(version.Value));
            }
            return base.StartingMembers(context, instance, members);
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

    public class ObjectReferencesExtension : WritingExtensionBase
    {
        private readonly IDictionary<string, object>
            _references = new ConcurrentDictionary<string, object>();

        private readonly ISerializationToolsFactory _factory;

        public ObjectReferencesExtension(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        protected override bool StartingMembers(IWriteContext context, object instance, IImmutableSet<MemberInfo> members)
        {
            var type = instance.GetType();
            var configuration = _factory.GetConfiguration(type);

            if (configuration?.IsObjectReference ?? false)
            {
                var objectId = configuration.GetObjectId(instance);
                var key = $"{type.FullName}{ExtendedXmlSerializer.Underscore}{objectId}";
                var contains = _references.ContainsKey(key);
                var property = contains ? (IAttachedProperty)new ObjectReferenceProperty(objectId) : new ObjectIdProperty(objectId);
                context.Attach(property);
                var result = !contains;
                if (result)
                {
                    _references.Add(key, instance);
                }
                return result;
            }
            return base.StartingMembers(context, instance, members);
        }

        protected override bool Initializing(IWriteContext context)
        {
            _references.Clear();
            return base.Initializing(context);
        }

        protected override void Completed(IWriteContext context) => _references.Clear();
    }
}