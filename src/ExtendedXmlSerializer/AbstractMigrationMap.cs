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
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization
{
    /// <summary>
    /// Base class for object migration map
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize</typeparam>
    public abstract class AbstractMigrationMap<T> : IMigrationMap<T>
    {

        /// <summary>
        /// Gets the dictionary of migartions
        /// </summary>
        public abstract Dictionary<int, Action<XElement>> Migrations { get; }

        /// <summary>
        /// Gets the version of object
        /// </summary>
        public abstract int Version { get;}

        /// <summary>
        /// Function to migrate node to current version
        /// </summary>
        /// <param name="targetType">The type of object</param>
        /// <param name="currentNode">The node</param>
        /// <param name="currentNodeVer">The current node version</param>
        public void Map(Type targetType, XElement currentNode, int currentNodeVer)
        {
            if (currentNodeVer > Version)
            {
                throw new XmlException($"Unknown varsion number {currentNodeVer} for type {targetType.FullName}.");
            }
            if (Migrations == null)
                throw new XmlException($"Dictionary of migrations for type {targetType.FullName} is null.");

            for (int i = currentNodeVer; i < Version; i++)
            {
                int versionNum = i;
                if (!Migrations.ContainsKey(i))
                    throw new XmlException($"Dictionary of migrations for type {targetType.FullName} does not contain {versionNum} migration.");
                if (Migrations[i] == null)
                    throw new XmlException($"Dictionary of migrations for type {targetType.FullName} contains invalid element in position {versionNum}.");
                Migrations[i](currentNode);
            }
        }
    }
}
