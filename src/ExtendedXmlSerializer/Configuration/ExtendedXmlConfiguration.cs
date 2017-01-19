using System;
using System.Collections.Generic;

namespace ExtendedXmlSerialization.Configuration
{
    public class ExtendedXmlConfiguration : IExtendedXmlConfiguration, IInternalExtendedXmlConfiguration
    {
        public bool AutoProperties { get; set; }
        public bool Namespaces { get; set; }
        public IPropertyEncryption EncryptionAlgorithm { get; set; }

        IExtendedXmlTypeConfiguration IInternalExtendedXmlConfiguration.GetTypeConfiguration(Type type)
        {
            return _cache.ContainsKey(type) ? _cache[type] : null;
        }

        private readonly Dictionary<Type, IExtendedXmlTypeConfiguration> _cache = new Dictionary<Type, IExtendedXmlTypeConfiguration>();

        public IExtendedXmlTypeConfiguration<T> ConfigureType<T>()
        {
            var configType = new ExtendedXmlTypeConfiguration<T>();

            _cache.Add(typeof(T), configType);
            return configType;
        }

        public IExtendedXmlConfiguration UseAutoProperties()
        {
            AutoProperties = true;
            return this;
        }

        public IExtendedXmlConfiguration UseNamespaces()
        {
            Namespaces = true;
            return this;
        }

        public IExtendedXmlConfiguration UseEncryptionAlgorithm(IPropertyEncryption propertyEncryption)
        {
            EncryptionAlgorithm = propertyEncryption;
            return this;
        }
    }
}