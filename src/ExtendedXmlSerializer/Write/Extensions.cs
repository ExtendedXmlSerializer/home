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

        public static WriteContext? GetValueContext(this IWritingContext @this)
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
            var defaultValue = _values(member.GetMemberType());
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
            if (current.Content != null)
            {
                return StartingContent(writing, current.Instance, current.Member, current.MemberValue, current.Content);
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
            if (current.Content != null)
            {
                FinishedContent(services, current.Instance, current.Member, current.MemberValue, current.Content);
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
                    if (current.Content != null)
                    {
                        return StartingContent(writing, configuration, current.Instance, current.Member, current.MemberValue, current.Content);
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
            
            return base.Starting(writing);
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

        protected override bool Initializing(IWriting services)
        {
            _references.Clear();
            return base.Initializing(services);
        }

        protected override bool StartingMemberValue(IWriting services, IExtendedXmlSerializerConfig configuration,
                                                    object instance,
                                                    MemberInfo member, object memberValue) =>
            Reference(services, For(member.GetMemberType()), memberValue);

        protected override bool StartingInstance(IWriting services, IExtendedXmlSerializerConfig configuration, object instance) => 
            Reference(services, configuration, instance);

        private bool Reference(IWriting services, IExtendedXmlSerializerConfig configuration, object instance)
        {
            if (configuration.IsObjectReference)
            {
                var objectId = configuration.GetObjectId(instance);
                var reference = _references.ContainsKey(instance);
                var property = reference ? (IAttachedProperty) new ObjectReferenceProperty(objectId) : new ObjectIdProperty(objectId);
                var result = !reference;
                services.Attach(property);
                if (result)
                {
                    
                    _references.Add(instance, instance);
                }
                else
                {
                    // services.Property(property);
                }
            }
            return false;
        }

        /* protected override bool StartingMember(IWriting services, IExtendedXmlSerializerConfig configuration, object instance, MemberInfo member)
        {
            var memberConfiguration = For(member.GetMemberType());
            if (memberConfiguration != null)
            {
                var value = Getters.Default.Get(member).Invoke(instance);
                if (value != null)
                {
                    var result = Store(services, memberConfiguration, value);
                    return result;
                }
            } 

            return base.StartingMember(services, configuration, instance, member);
        }*/

        protected override void Completed(IWriting services) => _references.Clear();
    }
}