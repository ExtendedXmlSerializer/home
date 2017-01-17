using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.NewConfiguration
{
    internal interface IExtendedXmlSerializerConfigType
    {
        IExtendedXmlSerializerConfigProperty GetPropertyConfig(string name);
        int Version { get; }
        void Map(Type targetType, XElement currentNode);
        object ReadObject(XElement element);
        void WriteObject(XmlWriter writer, object obj);

        bool IsCustomSerializer { get; set; }
    }

    internal class ExtendedXmlSerializerConfigType<T> : IExtendedXmlSerializerConfigType<T>, IExtendedXmlSerializerConfigType
    {
        /// <summary>
        /// Gets the dictionary of migartions
        /// </summary>
        private readonly Dictionary<int, Action<XElement>> _migrations = new Dictionary<int, Action<XElement>>();
        private Func<XElement, T> _deserialize;
        private Action<XmlWriter, T> _serializer;

        public IExtendedXmlSerializerConfigProperty GetPropertyConfig(string name)
        {
            return _cache.ContainsKey(name) ? _cache[name] : null;
        }

        /// <summary>
        /// Gets the version of object
        /// </summary>
        public int Version { get; private set; }
        public void Map(Type targetType, XElement currentNode)
        {
            int currentNodeVer = 0;
            var ver = currentNode.Attribute("ver");
            if (ver != null)
            {
                currentNodeVer = int.Parse(ver.Value);
            }
            if (currentNodeVer > Version)
            {
                throw new XmlException($"Unknown varsion number {currentNodeVer} for type {targetType.FullName}.");
            }
            if (_migrations == null)
                throw new XmlException($"Dictionary of migrations for type {targetType.FullName} is null.");

            for (int i = currentNodeVer; i < Version; i++)
            {
                int versionNum = i;
                if (!_migrations.ContainsKey(i))
                    throw new XmlException(
                        $"Dictionary of migrations for type {targetType.FullName} does not contain {versionNum} migration.");
                if (_migrations[i] == null)
                    throw new XmlException(
                        $"Dictionary of migrations for type {targetType.FullName} contains invalid element in position {versionNum}.");
                _migrations[i](currentNode);
            }
        }

        public object ReadObject(XElement element)
        {
            return _deserialize(element);
        }

        public void WriteObject(XmlWriter writer, object obj)
        {
            _serializer(writer, (T)obj);
        }


        public bool IsCustomSerializer { get; set; }

        private readonly Dictionary<string, IExtendedXmlSerializerConfigProperty> _cache = new Dictionary<string, IExtendedXmlSerializerConfigProperty>();
        public IExtendedXmlSerializerConfigProperty<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var propertyConfig = new ExtendedXmlSerializerConfigProperty<T, TProperty> {ConfigType = this};
            //TODO maybe something smarter.
            var path = property.Body.ToString();
            var binding = path.Substring(path.IndexOf(".", StringComparison.OrdinalIgnoreCase) + 1);
            
            _cache.Add(binding, propertyConfig);
            return propertyConfig;
        }

        public IExtendedXmlSerializerConfigType<T> CustomSerializer(Action<XmlWriter, T> serializer, Func<XElement, T> deserialize)
        {
            IsCustomSerializer = true;
            _serializer = serializer;
            _deserialize = deserialize;
            return this;
        }

        public IExtendedXmlSerializerConfigType<T> CustomSerializer(IExtendedXmlSerializerCustomSerializer<T> serializer)
        {
            IsCustomSerializer = true;
            _serializer = serializer.Serializer;
            _deserialize = serializer.Deserialize;
            return this;
        }

        public IExtendedXmlSerializerConfigType<T> AddMigration(Action<XElement> migration)
        {
            _migrations.Add(Version++, migration);
            return this;
        }

        public IExtendedXmlSerializerConfigType<T> AddMigration(IExtendedXmlSerializerTypeMigrator migrator)
        {
            foreach (var allMigration in migrator.GetAllMigrations())
            {
                _migrations.Add(Version++, allMigration);
            }
            return this;
        }
    }
}