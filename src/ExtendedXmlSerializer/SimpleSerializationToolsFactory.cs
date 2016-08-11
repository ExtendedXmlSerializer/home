// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
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