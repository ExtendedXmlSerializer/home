using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWritingExtension
    {
        bool Starting(IWriting writing);
        void Finished(IWriting services);
    }


    public static class Extensions
    {
        // public static TResult Accept<TParameter, TResult>( this TResult @this, TParameter _ ) => @this;

        public static void Property(this IWriting @this, IAttachedProperty property)
            => @this.Property(property.Name, @this.Serialize(property.Value));

        public static WriteContext? Parent(this IWritingContext @this, int level = 1) => @this.Hierarchy.ElementAtOrDefault(level);

        public static WriteContext? GetArrayContext(this IWritingContext @this)
        {
            var parent = @this.Parent((int) @this.Current.State);
            var instance = parent?.Instance;
            var result = instance != null && Arrays.Default.Is(instance) ? parent : null;
            return result;
        }

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
            MemberValueExistsExtension.Default,
            new ObjectReferencesExtension(factory),
            new VersionExtension(factory),
            new CustomSerializationExtension(factory)
        ) {}
    }

    class MemberValueExistsExtension : WritingExtensionBase
    {
        private readonly Func<Type, object> _values;
        public static MemberValueExistsExtension Default { get; } = new MemberValueExistsExtension();
        MemberValueExistsExtension() : this(DefaultValues.Default.Get) {}

        public MemberValueExistsExtension(Func<Type, object> values)
        {
            _values = values;
        }

        protected override bool StartingMember(IWriting services, object instance, MemberInfo member, object currentMemberValue)
        {
            var defaultValue = _values(services.Current.MemberType);
            var result = currentMemberValue != defaultValue;
            return result;
        }
    }

    class CompositeWritingExtension : CompositeServiceProvider, IWritingExtension
    {
        private readonly IEnumerable<IWritingExtension> _extensions;

        public CompositeWritingExtension(params IWritingExtension[] extensions) : this(extensions.ToImmutableList()) {}

        public CompositeWritingExtension(IEnumerable<IWritingExtension> extensions) : base(extensions)
        {
            _extensions = extensions;
        }

        public bool Starting(IWriting writing)
        {
            foreach (var extension in _extensions)
            {
                if (!extension.Starting(writing))
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

    /*public class DefaultWritingExtension : WritingExtensionBase
    {
        public static DefaultWritingExtension Default { get; } = new DefaultWritingExtension();
        DefaultWritingExtension() {}
    }*/

    public abstract class WritingExtensionBase : IWritingExtension
    {
        public virtual bool Starting(IWriting writing)
        {
            var current = writing.Current;
            if (current.Value != null)
            {
                return StartingContent(writing, current.Instance, current.Member, current.MemberValue, current.Value);
            }

            if (current.Member != null)
            {
                switch (current.State)
                {
                    case WriteState.MemberValue:
                        return StartingMemberValue(writing, current.Instance, current.Member, current.MemberValue);
                    default:
                        return StartingMember(writing, current.Instance, current.Member, current.MemberValue);
                }
            }

            if (current.Members != null)
            {
                return StartingMembers(writing, current.Instance, current.Members);
            }

            var result = current.Instance != null
                ? StartingInstance(writing, current.Instance)
                : Initializing(writing);
            return result;
        }

        protected virtual bool Initializing(IWriting services) => true;
        protected virtual bool StartingInstance(IWriting services, object instance) => true;
        protected virtual bool StartingMembers(IWriting services, object instance, IImmutableList<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWriting services, object instance, MemberInfo member, object currentMemberValue) => true;
        protected virtual bool StartingMemberValue(IWriting services, object instance, MemberInfo member, object memberValue) => true;
        protected virtual bool StartingContent(IWriting services, object instance, MemberInfo member, object memberValue, string content) => true;

        public virtual void Finished(IWriting services)
        {
            var current = services.Current;
            if (current.Value != null)
            {
                FinishedContent(services, current.Instance, current.Member, current.MemberValue, current.Value);
            }
            /*else if (current.HasValue)
            {
                FinishedMemberValue(services, current.Instance, current.Member, current.MemberValue);
            }*/
            else if (current.Member != null)
            {
                switch (current.State)
                {
                    case WriteState.MemberValue:
                        FinishedMemberValue(services, current.Instance, current.Member, current.MemberValue);
                        break;
                    default:
                        FinishedMember(services, current.Instance, current.Member);
                        break;
                }
                
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
        protected virtual void FinishedMemberValue(IWriting services, object instance, MemberInfo member, object memberValue) {}
        protected virtual void FinishedContent(IWriting services, object instance, MemberInfo member, object memberValue, string content) {}
        protected virtual void Completed(IWriting services) {}
    }

    public abstract class ConfigurationWritingExtensionBase : WritingExtensionBase
    {
        private readonly ISerializationToolsFactory _factory;

        protected ConfigurationWritingExtensionBase(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        protected IExtendedXmlSerializerConfig For(Type type) => _factory.GetConfiguration(type);

        public override bool Starting(IWriting writing)
        {
            var current = writing.Current;
            if (current.Instance != null)
            {
                var type = current.Instance.GetType();
                var configuration = For(type);
                if (configuration != null)
                {
                    if (current.Value != null)
                    {
                        return StartingContent(writing, configuration, current.Instance, current.Member, current.MemberValue, current.Value);
                    }

                    if (current.Member != null)
                    {
                        switch (current.State)
                        {
                            case WriteState.MemberValue:
                                return StartingMemberValue(writing, configuration, current.Instance, current.Member, current.MemberValue);
                                
                            default:
                                return StartingMember(writing, configuration, current.Instance, current.Member, current.MemberValue);
                        }
                    }

                    var result = current.Members != null
                        ? StartingMembers(writing, configuration, current.Instance, current.Members)
                        : StartingInstance(writing, configuration, current.Instance);
                    return result;
                }
            }

            var starting = base.Starting(writing);
            return starting;
        }

        protected virtual bool StartingInstance(IWriting services, IExtendedXmlSerializerConfig configuration,
                                                object instance) => true;
        protected virtual bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration,
                                               object instance, IImmutableList<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWriting services, IExtendedXmlSerializerConfig configuration, object instance, MemberInfo member, object currentMemberValue) => true;
        protected virtual bool StartingMemberValue(IWriting services, IExtendedXmlSerializerConfig configuration,
                                              object instance, MemberInfo member, object memberValue) => true;
        protected virtual bool StartingContent(IWriting services, IExtendedXmlSerializerConfig configuration,
                                               object instance, MemberInfo member, object memberValue, string content) => true;
    }

    public class CustomSerializationExtension : ConfigurationWritingExtensionBase
    {
        public CustomSerializationExtension(ISerializationToolsFactory factory) : base(factory) {}

        protected override bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration, object instance,
                                                IImmutableList<MemberInfo> members)
        {
            if (configuration.IsCustomSerializer)
            {
                EmitCurrentInstanceTypeInstruction.Default.Execute(services);
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
                EmitCurrentInstanceTypeInstruction.Default.Execute(services);
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
            _elements = new ConcurrentDictionary<object, object>(),
            _references = new ConcurrentDictionary<object, object>();

        public ObjectReferencesExtension(ISerializationToolsFactory factory) : base(factory) {}

        protected override bool Initializing(IWriting services)
        {
            Clear();
            return base.Initializing(services);
        }

        private void Clear()
        {
            _elements.Clear();
            _references.Clear();
        }

        protected override bool StartingInstance(IWriting services, object instance)
        {
            foreach (var item in Arrays.Default.AsArray(instance))
            {
                if (!_elements.ContainsKey(item))
                {
                    _elements.Add(item, item);
                }
            }
            return base.StartingInstance(services, instance);
        }

        protected override void FinishedInstance(IWriting services, object instance)
        {
            if (Arrays.Default.Is(instance))
            {
                _elements.Clear();
            }
            base.FinishedInstance(services, instance);
        }

        protected override bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration, object instance,
                                                IImmutableList<MemberInfo> members) => 
            Reference(services, configuration, instance, services.GetArrayContext() == null && _elements.ContainsKey(instance));

        private bool Reference(IWriting services, IExtendedXmlSerializerConfig configuration, object instance, bool force = false)
        {
            if (configuration?.IsObjectReference ?? false)
            {
                var objectId = configuration.GetObjectId(instance);
                var contains = _references.ContainsKey(instance);
                var reference = contains || force;
                var property = reference ? (IAttachedProperty) new ObjectReferenceProperty(objectId) : new ObjectIdProperty(objectId);
                var result = !reference;
                if (result)
                {
                    services.Attach(property);
                    _references.Add(instance, instance);
                }
                else
                {
                    services.Property(property);
                }
                return result;
            }
            return true;
        }

        protected override void Completed(IWriting services) => Clear();
    }
}