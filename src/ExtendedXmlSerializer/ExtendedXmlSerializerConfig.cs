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
    public interface IMigrationConfiguration<T>
    {
        IMigrationConfiguration<T> AddMigration(Action<XElement> migration);
    }

    public interface IObjectReferenceConfiguration<T>
    {
        void ExtractToList(string name);
    }


    public class ExtendedXmlSerializerConfig<T> : IMigrationConfiguration<T>, IObjectReferenceConfiguration<T>,
        IExtendedXmlSerializerConfig
    {
        public ExtendedXmlSerializerConfig()
        {
            _migrations = new Dictionary<int, Action<XElement>>();
            Version = 0;
        }

        /// <summary>
        /// Gets the type of object to deserialize
        /// </summary>
        public Type Type => typeof(T);


        /// <summary>
        /// Gets the dictionary of migartions
        /// </summary>
        private readonly Dictionary<int, Action<XElement>> _migrations;

        /// <summary>
        /// Gets the version of object
        /// </summary>
        public int Version { get; set; }

        private Func<XElement, T> _deserialize;
        private Action<XmlWriter, T> _serializer;
        private Func<T, object> getObjectId;

        public void CustomSerializer(Action<XmlWriter, T> serializer, Func<XElement, T> deserialize)
        {
            ((IExtendedXmlSerializerConfig) this).IsCustomSerializer = true;
            _serializer = serializer;
            _deserialize = deserialize;
        }

        public IMigrationConfiguration<T> AddMigration(Action<XElement> migration)
        {
            _migrations.Add(Version++, migration);
            return this;
        }

        public IObjectReferenceConfiguration<T> ObjectReference(Func<T, object> idFunc)
        {
            this.getObjectId = idFunc;
            ((IExtendedXmlSerializerConfig) this).IsObjectReference = true;
            return this;
        }

        void IObjectReferenceConfiguration<T>.ExtractToList(string name)
        {
            ((IExtendedXmlSerializerConfig) this).ExtractedListName = name;
        }

        void IExtendedXmlSerializerConfig.Map(Type targetType, XElement currentNode)
        {
            int currentNodeVer = 0;
            var serializeVersion = currentNode.Attribute("serializeVersion");
            if (serializeVersion != null)
            {
                currentNodeVer = int.Parse(serializeVersion.Value);
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


        object IExtendedXmlSerializerConfig.ReadObject(XElement element)
        {
            return _deserialize(element);
        }

        void IExtendedXmlSerializerConfig.WriteObject(XmlWriter writer, object obj)
        {
            _serializer(writer, (T) obj);
        }

        bool IExtendedXmlSerializerConfig.IsCustomSerializer { get; set; }
        bool IExtendedXmlSerializerConfig.IsObjectReference { get; set; }
        string IExtendedXmlSerializerConfig.ExtractedListName { get; set; }

        string IExtendedXmlSerializerConfig.GetObjectId(object obj)
        {
            return getObjectId((T) obj).ToString();
        }
    }
}