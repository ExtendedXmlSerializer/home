using System;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Configuration
{
    public interface IExtendedXmlSerializerConfigType<T>
    {
        IExtendedXmlSerializerConfigProperty<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> property);

        IExtendedXmlSerializerConfigType<T> CustomSerializer(Action<XmlWriter, T> serializer, Func<XElement, T> deserialize);
        IExtendedXmlSerializerConfigType<T> CustomSerializer(IExtendedXmlSerializerCustomSerializer<T> serializer);

        IExtendedXmlSerializerConfigType<T> AddMigration(Action<XElement> migration);
        IExtendedXmlSerializerConfigType<T> AddMigration(IExtendedXmlSerializerTypeMigrator migrator);
    }
}