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
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization
{
    public class ExtendedXmlSerializerConfig<T> : IMigrationConfiguration<T>, IObjectReferenceConfiguration<T>,
        IExtendedXmlSerializerConfig
    {
        private static readonly Func<Type, bool> Specification = type => typeof(T) == type;

        public ExtendedXmlSerializerConfig() : this(Specification) {}

        public ExtendedXmlSerializerConfig(Func<Type, bool> specification)
        {
            _migrations = new Dictionary<int, Action<XElement>>();
            Version = 0;
            _specification = specification;
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
        public int Version { get; private set; }

        private Func<XElement, T> _deserialize;
        private Action<XmlWriter, T> _serializer;
        private Func<T, object> _getObjectId;
        readonly Func<Type, bool> _specification;

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
            _getObjectId = idFunc;
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


        object IExtendedXmlSerializerConfig.ReadObject(XElement element)
        {
            return _deserialize(element);
        }

        void IExtendedXmlSerializerConfig.WriteObject(XmlWriter writer, object obj)
        {
            _serializer(writer, (T) obj);
        }

        public bool IsSatisfiedBy(Type type)
        {
            return _specification(type);
        }

        bool IExtendedXmlSerializerConfig.IsCustomSerializer { get; set; }
        bool IExtendedXmlSerializerConfig.IsObjectReference { get; set; }
        string IExtendedXmlSerializerConfig.ExtractedListName { get; set; }

        string IExtendedXmlSerializerConfig.GetObjectId(object obj)
        {
            return _getObjectId((T) obj).ToString();
        }

        bool IExtendedXmlSerializerConfig.CheckPropertyEncryption(string propertyName)
        {
            return PropertiesToEncrypt.Contains(propertyName);
        }

        public void Encrypt<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            MemberExpression member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{expression}' refers to a method, not a property.");

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{expression}' refers to a field, not a property.");

            if (Type != propInfo.DeclaringType && propInfo.DeclaringType != null &&
                !IsSatisfiedBy(propInfo.DeclaringType))
                throw new ArgumentException(
                    $"Expresion '{expression}' refers to a property that is not from type {Type}.");

            PropertiesToEncrypt.Add(propInfo.Name);
        }

        private List<string> PropertiesToEncrypt { get; set; } = new List<string>();
    }
}