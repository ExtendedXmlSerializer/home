using System;

namespace ExtendedXmlSerialization.Configuration
{
    internal interface IInternalExtendedXmlSerializerConfig
    {
        bool AutoProperties { get; set; }
        bool Namespaces { get; set; }
        IPropertyEncryption EncryptionAlgorithm { get; set; }
        IExtendedXmlSerializerConfigType GetTypeConfig(Type type);
    }
}