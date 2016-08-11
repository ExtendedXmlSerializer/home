using System;
using System.Collections.Generic;

namespace ExtendedXmlSerialization
{
    /// <summary>
    /// The simple implementation of <see cref="ISerializationToolsFactory"/>
    /// </summary>
    public class SimpleSerializationToolsFactory : ISerializationToolsFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ISerializationToolsFactory"/>
        /// </summary>
        public SimpleSerializationToolsFactory()
        {
            MigrationMaps = new List<IMigrationMap>();
            CustomSerializators = new List<ICustomSerializator>();
        }

        /// <summary>
        /// Gets or sets list of <see cref="IMigrationMap"/>
        /// </summary>
        public List<IMigrationMap> MigrationMaps { get; set; }

        /// <summary>
        /// Gets or sets list of <see cref="ICustomSerializator"/>
        /// </summary>
        public List<ICustomSerializator> CustomSerializators { get; set; }

        /// <summary>
        /// Gets <see cref="IMigrationMap" /> for particualr type
        /// </summary>
        /// <param name="type">The type of object to migration</param>
        /// <returns>The <see cref="IMigrationMap"/></returns>
        public IMigrationMap GetMigrationMap(Type type)
        {
            foreach (var migrationMap in MigrationMaps)
            {
                if (migrationMap.Type == type)
                {
                    return migrationMap;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets <see cref="ICustomSerializator"/> for particular type
        /// </summary>
        /// <param name="type">The type of object to serialization or deserialization</param>
        /// <returns>The <see cref="ICustomSerializator"/></returns>
        public ICustomSerializator GetCustomSerializer(Type type)
        {
            foreach (var customSerializer in CustomSerializators)
            {
                if (customSerializer.Type == type)
                {
                    return customSerializer;
                }
            }
            return null;

        }
    }
}