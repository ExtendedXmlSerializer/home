using System;
using System.Collections.Generic;

namespace ExtendedXmlSerialization.Configuration
{
    public class ExtendedXmlSerializerConfig : IExtendedXmlSerializerConfig, IInternalExtendedXmlSerializerConfig
    {
        public bool AutoProperties { get; set; }
        public bool Namespaces { get; set; }
        public IPropertyEncryption EncryptionAlgorithm { get; set; }

        IExtendedXmlSerializerConfigType IInternalExtendedXmlSerializerConfig.GetTypeConfig(Type type)
        {
            return _cache.ContainsKey(type) ? _cache[type] : null;
        }

        private readonly Dictionary<Type, IExtendedXmlSerializerConfigType> _cache = new Dictionary<Type, IExtendedXmlSerializerConfigType>();

        public IExtendedXmlSerializerConfigType<T> ConfigType<T>()
        {
            var configType = new ExtendedXmlSerializerConfigType<T>();

            _cache.Add(typeof(T), configType);
            return configType;
        }

        public IExtendedXmlSerializerConfig UseAutoProperties()
        {
            AutoProperties = true;
            return this;
        }

        public IExtendedXmlSerializerConfig UseNamespaces()
        {
            Namespaces = true;
            return this;
        }

        public IExtendedXmlSerializerConfig UseEncryptionAlgorithm(IPropertyEncryption propertyEncryption)
        {
            EncryptionAlgorithm = propertyEncryption;
            return this;
        }
    }
}