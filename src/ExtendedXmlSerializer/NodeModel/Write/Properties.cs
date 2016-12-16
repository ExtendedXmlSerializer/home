using System;

namespace ExtendedXmlSerialization.NodeModel.Write
{
    public class TypeProperty : Property
    {
        public TypeProperty(Type type) : base(new Primitive(type), typeof(IExtendedXmlSerializer), ExtendedXmlSerializer.Type) {}
    }

    /*
    public interface IAttachedProperties
    {
        void Attach(object instance, IProperty property);
        ICollection<IProperty> GetProperties(object instance);
    }
    class AttachedProperties : IAttachedProperties
    {
        public static AttachedProperties Default { get; } = new AttachedProperties();
        AttachedProperties() {}

        private readonly WeakCache<object, ICollection<IProperty>>
            _properties = new WeakCache<object, ICollection<IProperty>>(_ => new OrderedSet<IProperty>());

        public void Attach(object instance, IProperty property) => _properties.Get(instance).Add(property);
        public ICollection<IProperty> GetProperties(object instance) => _properties.Get(instance);
    }

    class ObjectReferenceProperty : PropertyBase
    {
        public ObjectReferenceProperty(Uri @namespace, string value)
            : base(@namespace, ExtendedXmlSerializer.Ref, value) {}
    }
        public sealed class PrimitiveNamespace : Namespace
    {
        private new const string Prefix = "sys";

        public new static PrimitiveNamespace Default { get; } = new PrimitiveNamespace();
        PrimitiveNamespace() : base(Prefix, new Uri("https://github.com/wojtpl2/ExtendedXmlSerializer/primitives")) {}
    }
        public sealed class RootNamespace : Namespace
    {
        private new const string Prefix = "exs";

        public RootNamespace(Uri identifier) : base(Prefix, identifier) {}
    }
    class VersionProperty : PropertyBase
    {
        public VersionProperty(Uri @namespace, int version) : base(@namespace, ExtendedXmlSerializer.Version, version) {}
    }
    class ObjectIdProperty : PropertyBase
    {
        public ObjectIdProperty(Uri @namespace, string value) : base(@namespace, ExtendedXmlSerializer.Id, value) {}
    }*/
}
