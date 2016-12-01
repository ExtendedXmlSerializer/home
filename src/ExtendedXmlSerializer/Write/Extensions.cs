using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWritingExtension
    {
        bool Starting(IWriting services);
        void Finished(IWriting services);
    }


    public static class Extensions
    {
        // public static TResult Accept<TParameter, TResult>( this TResult @this, TParameter _ ) => @this;

        public static void Property(this IWriting @this, IAttachedProperty property)
            => @this.Property(property.Name, @this.Serialize(property.Value));

        public static WriteContext? Parent(this IWritingContext @this) => @this.Hierarchy.ElementAtOrDefault(1);

        public static WriteContext? GetMemberContext(this IWritingContext @this)
        {
            if (@this.Current.Member != null)
            {
                return @this.Current;
            }
            var parent = @this.Parent();
            var result = parent?.Member != null ? parent : null;
            return result;
        }
    }

    class DefaultWritingExtensions : CompositeWritingExtension
    {
        public DefaultWritingExtensions(ISerializationToolsFactory factory) : base(
            new ObjectReferencesExtension(factory),
            new VersionExtension(factory),
            new CustomSerializationExtension(factory)
        ) {}
    }

    class CompositeWritingExtension : CompositeServiceProvider, IWritingExtension
    {
        private readonly IEnumerable<IWritingExtension> _extensions;

        public CompositeWritingExtension(params IWritingExtension[] extensions) : this(extensions.ToImmutableList()) {}

        public CompositeWritingExtension(IEnumerable<IWritingExtension> extensions) : base(extensions)
        {
            _extensions = extensions;
        }

        public bool Starting(IWriting services)
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

        public void Finished(IWriting services)
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

    class ObjectSerializer : IObjectSerializer
    {
        public static ObjectSerializer Default { get; } = new ObjectSerializer();
        ObjectSerializer() {}

        public string Serialize(object instance)
        {
            var result = instance as string ?? (instance as Enum)?.ToString() ?? PrimitiveValueTools.SetPrimitiveValue(instance);
            return result;
        }
    }

    public class DefaultWritingExtension : WritingExtensionBase
    {
        public static DefaultWritingExtension Default { get; } = new DefaultWritingExtension();
        DefaultWritingExtension() {}
    }

    public abstract class WritingExtensionBase : IWritingExtension
    {
        public virtual bool Starting(IWriting services)
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

        protected virtual bool Initializing(IWriting services) => true;
        protected virtual bool StartingInstance(IWriting services, object instance) => true;
        protected virtual bool StartingMembers(IWriting services, object instance, IImmutableList<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWriting services, object instance, MemberInfo member) => true;
        protected virtual bool StartingContent(IWriting services, object instance, MemberInfo member, string content) => true;

        public virtual void Finished(IWriting services)
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

        protected virtual void FinishedInstance(IWriting services, object instance) {}
        protected virtual void FinishedMembers(IWriting services, object instance, IImmutableList<MemberInfo> members) {}
        protected virtual void FinishedMember(IWriting services, object instance, MemberInfo member) {}
        protected virtual void FinishedContent(IWriting services, object instance, MemberInfo member, string content) {}
        protected virtual void Completed(IWriting services) {}
    }

    public abstract class ConfigurationWritingExtensionBase : WritingExtensionBase
    {
        private readonly ISerializationToolsFactory _factory;

        protected ConfigurationWritingExtensionBase(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        public override bool Starting(IWriting services)
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

        protected virtual bool StartingInstance(IWriting services, IExtendedXmlSerializerConfig configuration,
                                                object instance) => true;
        protected virtual bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration,
                                               object instance, IImmutableList<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWriting services, IExtendedXmlSerializerConfig configuration,
                                              object instance, MemberInfo member) => true;
        protected virtual bool StartingContent(IWriting services, IExtendedXmlSerializerConfig configuration,
                                               object instance, MemberInfo member, string content) => true;
    }

    public class CustomSerializationExtension : ConfigurationWritingExtensionBase
    {
        public CustomSerializationExtension(ISerializationToolsFactory factory) : base(factory) {}

        protected override bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration, object instance,
                                                IImmutableList<MemberInfo> members)
        {
            if (configuration.IsCustomSerializer)
            {
                configuration.WriteObject(services.Get<XmlWriter>(), instance);
                return false;
            }
            return base.StartingMembers(services, configuration, instance, members);	
        }
    }

    public class VersionExtension : ConfigurationWritingExtensionBase
    {
        public VersionExtension(ISerializationToolsFactory factory) : base(factory) {}

        protected override bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration, object instance,
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

        protected override bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration, object instance,
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

        protected override bool Initializing(IWriting services)
        {
            _references.Clear();
            return base.Initializing(services);
        }

        protected override void Completed(IWriting services) => _references.Clear();
    }
}