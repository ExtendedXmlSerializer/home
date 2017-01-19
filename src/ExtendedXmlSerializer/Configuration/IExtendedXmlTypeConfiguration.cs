using System;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Configuration
{
    public interface IExtendedXmlTypeConfiguration<T>
    {
        IExtendedXmlPropertyConfiguration<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> property);

        IExtendedXmlTypeConfiguration<T> CustomSerializer(Action<XmlWriter, T> serializer, Func<XElement, T> deserialize);
        IExtendedXmlTypeConfiguration<T> CustomSerializer(IExtendedXmlCustomSerializer<T> serializer);

        IExtendedXmlTypeConfiguration<T> AddMigration(Action<XElement> migration);
        IExtendedXmlTypeConfiguration<T> AddMigration(IExtendedXmlTypeMigrator migrator);
    }
    internal interface IExtendedXmlTypeConfiguration
    {
        IExtendedXmlPropertyConfiguration GetPropertyConfiguration(string name);
        int Version { get; }
        void Map(Type targetType, XElement currentNode);
        object ReadObject(XElement element);
        void WriteObject(XmlWriter writer, object obj);

        bool IsCustomSerializer { get; set; }
        bool IsObjectReference { get; set; }
        Func<object, string> GetObjectId { get; set; }
    }
}