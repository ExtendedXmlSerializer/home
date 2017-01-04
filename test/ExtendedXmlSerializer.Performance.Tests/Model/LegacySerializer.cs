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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Model;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Performance.Tests.Model
{
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

    namespace ExtendedXmlSerialization
    {
        /// <summary>
        /// Extended Xml Serializer
        /// </summary>
        public class LegacyXmlSerializer : IExtendedXmlSerializer
        {
            const string Type = "type";
            const string Ref = "ref";
            const string Version = "ver";
            const string Id = "id";
            const string Key = "Key";
            const string Value = "Value";
            const string Underscore = "_";
            const string Item = "Item";
            private ISerializationToolsFactory _toolsFactory;

            private readonly Dictionary<string, object> _referencesObjects = new Dictionary<string, object>();
            private readonly Dictionary<string, object> _reservedReferencesObjects = new Dictionary<string, object>();

            /// <summary>
            /// Creates an instance of <see cref="LegacyXmlSerializer"/>
            /// </summary>
            public LegacyXmlSerializer() {}

            /// <summary>
            /// Creates an instance of <see cref="LegacyXmlSerializer"/>
            /// </summary>
            /// <param name="toolsFactory">The instance of <see cref="ISerializationToolsFactory"/></param>
            public LegacyXmlSerializer(ISerializationToolsFactory toolsFactory)
            {
                _toolsFactory = toolsFactory;
            }

            /// <summary>
            /// Gets or sets <see cref="ISerializationToolsFactory"/>
            /// </summary>
            public ISerializationToolsFactory SerializationToolsFactory
            {
                get { return _toolsFactory; }
                set { _toolsFactory = value; }
            }

            /// <summary>
            /// Serializes the specified <see cref="T:System.Object" /> and returns xml document in string
            /// </summary>
            /// <param name="o">The <see cref="T:System.Object" /> to serialize. </param>
            /// <returns>xml document in string</returns>
            public string Serialize(object o)
            {
                var def = TypeDefinitions.Default.Get(o.GetType());

                string xml;
                using (var ms = new MemoryStream())
                {
                    using (XmlWriter xw = XmlWriter.Create(ms))
                    {
                        WriteXml(xw, o, def);
                    }
                    ms.Position = 0;

                    using (var sr = new StreamReader(ms))
                    {
                        xml = sr.ReadToEnd();
                    }
                }

                _referencesObjects.Clear();
                return xml;
            }

            private void WriteXmlDictionary(object o, XmlWriter writer, ITypeDefinition def, string name,
                                            bool forceSaveType)
            {
                writer.WriteStartElement(name ?? def.Name);
                if (forceSaveType)
                {
                    writer.WriteAttributeString(Type, def.Type.FullName);
                }
                var dict = o as IDictionary;
                if (dict != null)
                {
                    foreach (DictionaryEntry item in dict)
                    {
                        writer.WriteStartElement(Item);

                        var itemDef = TypeDefinitions.Default.Get(item.Key.GetType());
                        WriteXml(writer, item.Key, itemDef, Key,
                                 forceSaveType: def.GenericArguments[0].FullName != itemDef.Type.FullName);

                        var itemValueDef = TypeDefinitions.Default.Get(item.Value.GetType());
                        WriteXml(writer, item.Value, itemValueDef, Value,
                                 forceSaveType: def.GenericArguments[1].FullName != itemValueDef.Type.FullName);

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }

            private void WriteXmlArray(object o, XmlWriter writer, IDefinition def, string name, bool forceSaveType)
            {
                writer.WriteStartElement(name ?? def.Name);
                if (forceSaveType)
                {
                    writer.WriteAttributeString(Type, def.Type.FullName);
                }
                List<string> toWriteReservedObject = new List<string>();
                var elementType = ElementTypeLocator.Default.Locate(def.Type);
                if (elementType != null)
                {
                    var enumerable = o as IEnumerable;
                    if (enumerable != null)
                    {
                        var items = enumerable as Array ?? enumerable.Cast<object>().ToArray();
                        var conf = GetConfiguration(elementType);
                        if (conf != null && conf.IsObjectReference)
                        {
                            foreach (var item in items)
                            {
                                var objectId = conf.GetObjectId(item);

                                var key = item.GetType().FullName + Underscore + objectId;
                                if (!_referencesObjects.ContainsKey(key) && !_reservedReferencesObjects.ContainsKey(key))
                                {
                                    toWriteReservedObject.Add(key);
                                    _reservedReferencesObjects.Add(key, item);
                                }
                            }
                        }

                        foreach (var item in items)
                        {
                            var itemDef = TypeDefinitions.Default.Get(item.GetType());
                            var writeReservedObject = false;
                            if (conf != null && conf.IsObjectReference)
                            {
                                var objectId = conf.GetObjectId(item);
                                var key = item.GetType() + Underscore + objectId;
                                if (toWriteReservedObject.Contains(key))
                                {
                                    writeReservedObject = true;
                                }
                            }
                            WriteXml(writer, item, itemDef, writeReservedObject: writeReservedObject,
                                     forceSaveType: elementType.FullName != itemDef.Type.FullName);
                        }
                    }
                }
                writer.WriteEndElement();
            }

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
                    return GetPrimitiveValue(type, currentNode.Value, currentNode.Name.LocalName);
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
                var configuration = GetConfiguration(currentNodeDef.Type);
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
                        if (!string.IsNullOrEmpty(refId))
                        {
                            var key = currentNodeDef.Type.FullName + Underscore + refId;
                            if (_referencesObjects.ContainsKey(key))
                            {
                                return _referencesObjects[key];
                            }
                            _referencesObjects.Add(key, currentObject);
                        }
                        string objectId = currentNode.Attribute(Id)?.Value;
                        if (!string.IsNullOrEmpty(objectId))
                        {
                            var key = currentNodeDef.Type.FullName + Underscore + objectId;
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
                    else if (propertyDef.IsEnumerable)
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
                                var algorithm = GetEncryptionAlgorithm();
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

            private static object GetPrimitiveValue(ITypeDefinition type, string value, string name)
            {
                try
                {
                    return type.Convert(value);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                              $"Unsuccessful conversion node {name} for type {type.Name} - value {value}", ex);
                }
            }

            private object ReadXmlDictionary(XElement currentNode, ITypeDefinition type, object instance = null)
            {
                int arrayCount = currentNode.Elements().Count();
                var elements = currentNode.Elements().ToArray();

                var definition = GetElementTypeDefinition(currentNode) ?? type;
                object dict = instance ?? definition.Activate();

                for (int i = 0; i < arrayCount; i++)
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
                var isArray = instance is Array;
                if (isArray)
                {
                    array = Array.CreateInstance(type.Type.GetElementType(), arrayCount);
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

            private ITypeDefinition GetElementTypeDefinition(XElement element, Type defuaultType = null)
            {
                var typeAttribute = element.Attribute(Type);
                if (typeAttribute != null)
                {
                    return TypeDefinitions.Default.Get(Types.Default.Get(typeAttribute.Value));
                }
                return defuaultType == null ? null : TypeDefinitions.Default.Get(defuaultType);
            }

            private void WriteXmlPrimitive(object o, XmlWriter xw, ITypeDefinition def, string name = null,
                                           bool toEncrypt = false, string valueType = null)
            {
                xw.WriteStartElement(name ?? def.Name);
                if (!string.IsNullOrEmpty(valueType))
                {
                    xw.WriteAttributeString(Type, valueType);
                }
                var value = ValueServices.AsString(o);
                if (toEncrypt)
                {
                    var algorithm = GetEncryptionAlgorithm();
                    if (algorithm != null)
                    {
                        value = algorithm.Encrypt(value);
                    }
                }
                xw.WriteString(value);
                xw.WriteEndElement();
            }

            private void WriteXml(XmlWriter writer, object o, ITypeDefinition type, string name = null,
                                  bool writeReservedObject = false, bool forceSaveType = false)
            {
                if (type.IsPrimitive)
                {
                    WriteXmlPrimitive(o, writer, type, name);
                    return;
                }
                if (type.IsDictionary)
                {
                    WriteXmlDictionary(o, writer, type, name, forceSaveType);
                    return;
                }
                if (type.IsEnumerable)
                {
                    WriteXmlArray(o, writer, type, name, forceSaveType);
                    return;
                }
                writer.WriteStartElement(name ?? type.Name);

                // Get configuration for type
                var configuration = GetConfiguration(type.Type);

                var fullName = type.Type.FullName;
                if (configuration != null)
                {
                    if (configuration.IsObjectReference)
                    {
                        var objectId = configuration.GetObjectId(o);

                        var key = fullName + Underscore + objectId;
                        if (writeReservedObject && _reservedReferencesObjects.ContainsKey(key))
                        {
                            _reservedReferencesObjects.Remove(key);
                        }
                        else if (_referencesObjects.ContainsKey(key) || _reservedReferencesObjects.ContainsKey(key))
                        {
                            if (forceSaveType)
                            {
                                writer.WriteAttributeString(Type, fullName);
                            }

                            writer.WriteAttributeString(Ref, objectId);
                            writer.WriteEndElement();
                            return;
                        }
                        writer.WriteAttributeString(Type, fullName);
                        writer.WriteAttributeString(Id, objectId);
                        _referencesObjects.Add(key, o);
                    }
                    else
                    {
                        writer.WriteAttributeString(Type, fullName);
                    }

                    if (configuration.Version > 0)
                    {
                        writer.WriteAttributeString(Version,
                                                    configuration.Version.ToString(CultureInfo.InvariantCulture));
                    }
                    if (configuration.IsCustomSerializer)
                    {
                        configuration.WriteObject(writer, o);
                        writer.WriteEndElement();
                        return;
                    }
                }
                else
                {
                    writer.WriteAttributeString(Type, fullName);
                }

                var properties = type.Members;
                foreach (var propertyInfo in properties)
                {
                    var propertyValue = propertyInfo.GetValue(o);
                    if (propertyValue == null)
                        continue;

                    var defType = TypeDefinitions.Default.Get(propertyValue.GetType());

                    if (defType.IsEnumerable)
                    {
                        WriteXml(writer, propertyValue, defType, propertyInfo.Name,
                                 forceSaveType:
                                 propertyInfo.IsWritable && propertyInfo.TypeDefinition.Type.FullName != defType.Type.FullName);
                    }
                    else if (defType.Type.GetTypeInfo().IsEnum)
                    {
                        writer.WriteStartElement(propertyInfo.Name);
                        writer.WriteString(propertyValue.ToString());
                        writer.WriteEndElement();
                    }
                    else
                    {
                        bool toEncrypt = false;
                        if (configuration != null)
                        {
                            if (configuration.CheckPropertyEncryption(propertyInfo.Name))
                            {
                                toEncrypt = true;
                            }
                        }
                        if (propertyInfo.TypeDefinition.Type.FullName != defType.Type.FullName)
                        {
                            WriteXmlPrimitive(propertyValue, writer, defType, propertyInfo.Name, toEncrypt,
                                              defType.Type.FullName);
                        }
                        else
                        {
                            WriteXmlPrimitive(propertyValue, writer, defType, propertyInfo.Name, toEncrypt);
                        }
                    }
                }
                writer.WriteEndElement();
            }

            private IExtendedXmlSerializerConfig GetConfiguration(Type type)
            {
                return _toolsFactory?.GetConfiguration(type);
            }

            private IPropertyEncryption GetEncryptionAlgorithm()
            {
                return _toolsFactory?.EncryptionAlgorithm;
            }
        }
    }
}