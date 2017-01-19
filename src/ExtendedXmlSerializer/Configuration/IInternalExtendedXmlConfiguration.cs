using System;

namespace ExtendedXmlSerialization.Configuration
{
    internal interface IInternalExtendedXmlConfiguration
    {
        bool AutoProperties { get; set; }
        bool Namespaces { get; set; }
        IPropertyEncryption EncryptionAlgorithm { get; set; }
        IExtendedXmlTypeConfiguration GetTypeConfiguration(Type type);
    }
}