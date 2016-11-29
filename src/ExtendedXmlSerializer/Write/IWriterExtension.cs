using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ExtendedXmlSerialization.Write
{
    public interface IWriterExtension
    {
        void Started(IWriteServices services);
        void Finished(IWriteServices services);

        bool Started(IWriteServices services, object instance);
        void Finished(IWriteServices services, object instance);

        bool Started(IWriteServices services, object instance, string content);
        void Finished(IWriteServices services, object instance, string content);

        bool Started(IWriteServices services, object instance, MemberInfo member);
        void Finished(IWriteServices services, object instance, MemberInfo member);

        bool Started(IWriteServices services, object instance, MemberInfo member, string content);
        void Finished(IWriteServices services, object instance, MemberInfo member, string content);
    }

    class EmitContentInstructionExtended : WriteInstructionBase
    {
        public static EmitContentInstructionExtended Default { get; } = new EmitContentInstructionExtended();
        EmitContentInstructionExtended() {}

        protected override void Execute(IWriteServices services, object instance)
        {
            var content = services.Serialize(instance);
            if (services.Started(services, instance, content))
            {
                services.Emit(content);
            }
            services.Finished(services, instance, content);
        }
    }

    class EmitMemberContentInstructionExtended : WriteInstructionBase
    {
        private readonly MemberInfo _member;

        public EmitMemberContentInstructionExtended(MemberInfo member)
        {
            _member = member;
        }

        protected override void Execute(IWriteServices services, object instance)
        {
            var content = services.Serialize(instance);
            if (services.Started(services, instance, _member, content))
            {
                services.Member(_member.Name, content);
            }
            services.Finished(services, instance, _member, content);
        }
    }

    class EmitMemberInstructionExtended : DecoratedWriteInstruction<object>
    {
        private readonly MemberInfo _member;
        public EmitMemberInstructionExtended(MemberInfo member, IInstruction instruction) : base(instruction)
        {
            _member = member;
        }

        protected override void Execute(IWriteServices services, object instance)
        {
            if (services.Started(services, instance, _member))
            {
                base.Execute(services, instance);
            }
            services.Finished(services, instance, _member);
        }
    }

    class EmitObjectInstructionExtended : DecoratedWriteInstruction<object>
    {
        public EmitObjectInstructionExtended(IInstruction instruction) : base(instruction) {}

        protected override void Execute(IWriteServices services, object instance)
        {
            if (services.Started(services, instance))
            {
                base.Execute(services, instance);
            }
            services.Finished(services, instance);
        }
    }

    class ToolingObjectSerializer : IObjectSerializer
    {
        private readonly Type _type;

        public ToolingObjectSerializer(Type type)
        {
            _type = type;
        }

        public string Serialize(object instance) => PrimitiveValueTools.SetPrimitiveValue(instance, _type);
    }

    public class DefaultWritingExtension : WritingExtensionBase
    {
        public static DefaultWritingExtension Default { get; } = new DefaultWritingExtension();
        DefaultWritingExtension() {}
    }

    public abstract class WritingExtensionBase : IWriterExtension
    {
        public virtual void Started(IWriteServices services) {}
        public virtual void Finished(IWriteServices services) {}

        public virtual bool Started(IWriteServices services, object instance) => true;
        public virtual void Finished(IWriteServices services, object instance) {}

        public virtual bool Started(IWriteServices services, object instance, string content) => true;
        public virtual void Finished(IWriteServices services, object instance, string content) {}

        public virtual bool Started(IWriteServices services, object instance, MemberInfo member) => true;
        public virtual void Finished(IWriteServices services, object instance, MemberInfo member) {}

        public bool Started(IWriteServices services, object instance, MemberInfo member, string content) => true;
        public void Finished(IWriteServices services, object instance, MemberInfo member, string content) {}
    }

    public class VersionExtension : WritingExtensionBase
    {
        private readonly IExtendedXmlSerializer _serializer;

        public VersionExtension(IExtendedXmlSerializer serializer)
        {
            _serializer = serializer;
        }

        public override bool Started(IWriteServices services, object instance)
        {
            var type = instance.GetType();
            var version = _serializer
                .SerializationToolsFactory?
                .GetConfiguration(type)?
                .Version;

            if (version != null)
            {
                services.Member(ExtendedXmlSerializer.Version,
                                version.Value.ToString(CultureInfo.InvariantCulture));
            }
            return true;
        }
    }

    public class ObjectReferencesExtension : WritingExtensionBase
    {
        private readonly IDictionary<string, object>
            _references = new ConcurrentDictionary<string, object>();

        private readonly IExtendedXmlSerializer _serializer;

        public ObjectReferencesExtension(IExtendedXmlSerializer serializer)
        {
            _serializer = serializer;
        }

        public override void Started(IWriteServices services) => _references.Clear();

        public override bool Started(IWriteServices services, object instance)
        {
             var type = instance.GetType();
            var configuration = _serializer.SerializationToolsFactory?.GetConfiguration(type);

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

            return true;
        }

        public override void Finished(IWriteServices services) => _references.Clear();
    }
}