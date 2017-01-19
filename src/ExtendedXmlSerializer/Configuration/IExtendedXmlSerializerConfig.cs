namespace ExtendedXmlSerialization.Configuration
{
    public interface IExtendedXmlSerializerConfig
    {
        IExtendedXmlSerializerConfigType<T> ConfigType<T>();
        IExtendedXmlSerializerConfig UseAutoProperties();
        IExtendedXmlSerializerConfig UseNamespaces();
        IExtendedXmlSerializerConfig UseEncryptionAlgorithm(IPropertyEncryption propertyEncryption);
    }
}
