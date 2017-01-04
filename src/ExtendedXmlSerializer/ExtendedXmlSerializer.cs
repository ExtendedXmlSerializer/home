// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
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
using System.Linq;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Model;
using ExtendedXmlSerialization.Processing;
using ExtendedXmlSerialization.Processing.Write;
using ISerializer = ExtendedXmlSerialization.Processing.Write.ISerializer;

namespace ExtendedXmlSerialization
{
    /// <summary>
    /// Extended Xml Serializer
    /// </summary>
    public class ExtendedXmlSerializer : IExtendedXmlSerializer
    {
        public const string Type = "type";
        public const string Ref = "ref";
        public const string Version = "ver";
        public const string Id = "id";
        public const string Key = "Key";
        public const string Value = "Value";
        public const string Underscore = "_";
        public const string Item = "Item";

        private readonly Dictionary<string, object> _referencesObjects = new Dictionary<string, object>();
        private ISerializationToolsFactory _tools;

        public ExtendedXmlSerializer() : this(null) {}

        public ExtendedXmlSerializer(ISerializationToolsFactory toolsFactory)
        {
            SerializationToolsFactory = toolsFactory;
        }

        /// <summary>
        /// Gets or sets <see cref="ISerializationToolsFactory"/>
        /// </summary>
        public ISerializationToolsFactory SerializationToolsFactory
        {
            get { return _tools; }
            set
            {
                _tools = value;
                Serializer = _tools != null
                    ? new LegacySerializer(_tools)
                    : (ISerializer) SimpleSerializer.Default;
            }
        }
        
        private ISerializer Serializer { get; set; } = SimpleSerializer.Default;

        /// <summary>
        /// Serializes the specified <see cref="T:System.Object" /> and returns xml document in string
        /// </summary>
        /// <param name="o">The <see cref="T:System.Object" /> to serialize. </param>
        /// <returns>xml document in string</returns>
        public string Serialize(object o) => Serializer.Serialize(o);

        /// <summary>
        /// Deserializes the XML document
        /// </summary>
        /// <param name="xml">The XML document</param>
        /// <param name="type">The type of returned object</param>
        /// <returns>deserialized object</returns>
        public object Deserialize(string xml, Type type)
        {
            var def = TypeDefinitions.Default.Get(type);
            XDocument doc = XDocument.Parse(xml);
            var result = ReadXml(doc.Root, def);
            _referencesObjects.Clear();
            return result;
        }

        /// <summary>
        /// Deserializes the XML document
        /// </summary>
        /// <typeparam name="T">The type of returned object</typeparam>
        /// <param name="xml">The XML document</param>
        /// <returns>deserialized object</returns>
        public T Deserialize<T>(string xml)
        {
            return (T) Deserialize(xml, typeof(T));
        }

        private object ReadXml(XElement currentNode, ITypeDefinition type, object instance = null)
        {
            if (type.IsPrimitive)
            {
                return GetPrimitiveValue(type, currentNode);
            }
            if (type.IsDictionary)
            {
                return ReadXmlDictionary(currentNode, type);
            }

            if (type.IsEnumerable)
            {
                return ReadXmlArray(currentNode, type, instance);
            }

            if (currentNode == null)
                return null;

            // Retrieve type from XML (Property can be base type. In xml can be saved inherited object) or type of get property
            var currentNodeDef = GetElementTypeDefinition(currentNode) ?? type;

            // Get configuration for type
            var configuration = SerializationToolsFactory?.GetConfiguration(currentNodeDef.Type);
            if (configuration != null)
            {
                // Run migrator if exists
                if (configuration.Version > 0)
                {
                    configuration.Map(currentNodeDef.Type, currentNode);
                }
                // run custom serializer if exists
                if (configuration.IsCustomSerializer)
                {
                    return configuration.ReadObject(currentNode);
                }
            }

            // Create new instance if not exists
            var currentObject = instance ?? currentNodeDef.Activate();

            if (configuration != null)
            {
                if (configuration.IsObjectReference)
                {
                    string refId = currentNode.Attribute(Ref)?.Value;
                    var prefix = currentNodeDef.Type.FullName + Underscore;
                    if (!string.IsNullOrEmpty(refId))
                    {
                        var key = prefix + refId;
                        if (_referencesObjects.ContainsKey(key))
                        {
                            return _referencesObjects[key];
                        }
                        _referencesObjects.Add(key, currentObject);
                    }
                    string objectId = currentNode.Attribute(Id)?.Value;
                    if (!string.IsNullOrEmpty(objectId))
                    {
                        var key = prefix + objectId;
                        if (_referencesObjects.ContainsKey(key))
                        {
                            currentObject = _referencesObjects[key];
                        }
                        else
                        {
                            _referencesObjects.Add(key, currentObject);
                        }
                    }
                }
            }

            // Read all elements
            foreach (var xElement in currentNode.Elements())
            {
                var localName = xElement.Name.LocalName;
                var value = xElement.Value;
                var propertyInfo = type[localName];
                if (propertyInfo == null)
                {
                    throw new InvalidOperationException("Missing property " + currentNode.Name.LocalName + "\\" +
                                                        localName);
                }
                var propertyDef = propertyInfo.TypeDefinition;
                if (xElement.HasAttributes && xElement.Attribute(Type) != null)
                {
                    // If type of property is saved in xml, we need check type of object actual assigned to property. There may be a base type. 
                    Type targetType = Types.Default.Get(xElement.Attribute(Type).Value);
                    var targetTypeDef = TypeDefinitions.Default.Get(targetType);
                    var obj = propertyInfo.GetValue(currentObject);
                    if ((obj == null || obj.GetType() != targetType) && targetTypeDef.CanActivate)
                    {
                        obj = targetTypeDef.Activate();
                    }
                    var obj2 = ReadXml(xElement, targetTypeDef, obj);
                    propertyInfo.SetValue(currentObject, obj2);
                }
                else if (propertyDef.IsEnumerable || currentObject is Array || propertyDef.Members.Any())
                {
                    //If xml does not contain type but we known that it is object
                    var obj = propertyInfo.GetValue(currentObject);
                    var obj2 = ReadXml(xElement, propertyDef, obj);
                    if (obj == null && obj2 != null)
                    {
                        propertyInfo.SetValue(currentObject, obj2);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    if (configuration != null)
                    {
                        if (configuration.CheckPropertyEncryption(propertyInfo.Name))
                        {
                            var algorithm = SerializationToolsFactory?.EncryptionAlgorithm;
                            if (algorithm != null)
                            {
                                value = algorithm.Decrypt(value);
                            }
                        }
                    }

                    object primitive = GetPrimitiveValue(propertyDef, value, xElement.Name.LocalName);
                    propertyInfo.SetValue(currentObject, primitive);
                }
            }
            return currentObject;
        }

        private static object GetPrimitiveValue(ITypeDefinition type, XElement currentNode)
            => GetPrimitiveValue(type, currentNode.Value, currentNode.Name.LocalName);

        private static object GetPrimitiveValue(ITypeDefinition type, string value, string name)
        {
            try
            {
                return PrimitiveValueTools.GetPrimitiveValue(value, type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                          $"Unsuccessful conversion node {name} for type {type.Name} - value {value}", ex);
            }
        }

        private object ReadXmlDictionary(XElement currentNode, ITypeDefinition type, object instance = null)
        {
            var elements = currentNode.Elements().ToArray();
            var count = elements.Length;

            var definition = GetElementTypeDefinition(currentNode) ?? type;
            object dict = instance ?? definition.Activate();

            for (int i = 0; i < count; i++)
            {
                var element = elements[i];

                var key = element.Element(Key);
                var value = element.Element(Value);

                var keyDef = GetElementTypeDefinition(key, type.GenericArguments[0]);
                var valuDef = GetElementTypeDefinition(value, type.GenericArguments[1]);

                type.Add(dict, ReadXml(key, keyDef), ReadXml(value, valuDef));
            }
            return dict;
        }

        private object ReadXmlArray(XElement currentNode, ITypeDefinition type, object instance = null)
        {
            var elements = currentNode.Elements().ToArray();
            int arrayCount = elements.Length;
            object list = null;
            Array array = null;
            var isArray = type.Type.IsArray;
            if (isArray)
            {
                array = instance as Array ?? Array.CreateInstance(type.Type.GetElementType(), arrayCount);
            }
            else
            {
                var definition = GetElementTypeDefinition(currentNode) ?? type;
                list = instance ?? definition.Activate();
            }

            var elementType = ElementTypeLocator.Default.Locate(type.Type);
            for (int i = 0; i < arrayCount; i++)
            {
                var element = elements[i];
                var definition = GetElementTypeDefinition(element, elementType);

                var xml = ReadXml(element, definition);
                if (isArray)
                {
                    array.SetValue(xml, i);
                }
                else
                {
                    type.Add(list, xml);
                }
            }
            return isArray ? array : list;
        }

        private static ITypeDefinition GetElementTypeDefinition(XElement element, Type defuaultType = null)
        {
            var value = element.Attribute(Type)?.Value;
            var type = value != null ? Types.Default.Get(value) : defuaultType;
            var result = type != null ? TypeDefinitions.Default.Get(type) : null;
            return result;
        }
    }
}