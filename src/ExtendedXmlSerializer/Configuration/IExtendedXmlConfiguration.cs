namespace ExtendedXmlSerialization.Configuration
{
    public interface IExtendedXmlConfiguration
    {
        IExtendedXmlTypeConfiguration<T> ConfigureType<T>();
        IExtendedXmlConfiguration UseAutoProperties();
        IExtendedXmlConfiguration UseNamespaces();
        IExtendedXmlConfiguration UseEncryptionAlgorithm(IPropertyEncryption propertyEncryption);
    }
}
