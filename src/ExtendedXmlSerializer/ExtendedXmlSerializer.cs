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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;
using ExtendedXmlSerialization.Write;

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
        private ISerializationToolsFactory _toolsFactory;
        
        private readonly Dictionary<string, object> _referencesObjects = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _reservedReferencesObjects = new Dictionary<string, object>();
        
        /// <summary>
        /// Creates an instance of <see cref="ExtendedXmlSerializer"/>
        /// </summary>
        public ExtendedXmlSerializer()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="ExtendedXmlSerializer"/>
        /// </summary>
        /// <param name="toolsFactory">The instance of <see cref="ISerializationToolsFactory"/></param>
        public ExtendedXmlSerializer(ISerializationToolsFactory toolsFactory)
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
            /*var def = TypeDefinitionCache.GetDefinition(o.GetType());

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
                _referencesObjects.Clear();
            }*/

            var serializer = SerializationToolsFactory != null ? new Serializer(new DefaultWriteExtensions(SerializationToolsFactory)) : Serializer.Default;
            var result = serializer.Serialize(o);
            return result;
        }

        private void WriteXmlDictionary(object o, XmlWriter writer, TypeDefinition def, string name, bool forceSaveType)
        {
            writer.WriteStartElement(name ?? def.Name);
            if (forceSaveType)
            {
                writer.WriteAttributeString(Type, def.FullName);
            }
            var dict = o as IDictionary;
            if (dict != null)
            {
                foreach (DictionaryEntry item in dict)
                {
                    writer.WriteStartElement(Item);
   
                    var itemDef = TypeDefinitionCache.GetDefinition(item.Key.GetType());
                    WriteXml(writer, item.Key, itemDef, Key);

                    var itemValueDef = TypeDefinitionCache.GetDefinition(item.Value.GetType());
                    WriteXml(writer, item.Value, itemValueDef, Value);

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        private void WriteXmlArray(object o, XmlWriter writer, TypeDefinition def, string name, bool forceSaveType)
        {
            writer.WriteStartElement(name ?? def.Name);
            if (forceSaveType)
            {
                writer.WriteAttributeString(Type, def.FullName);
            }
            List<string> toWriteReservedObject = new List<string>();
            var elementType = ElementTypeLocator.Default.Locate( def.Type );
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
                        var itemDef = TypeDefinitionCache.GetDefinition(item.GetType());
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
                        WriteXml(writer, item, itemDef, writeReservedObject: writeReservedObject);
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
            var def = TypeDefinitionCache.GetDefinition(type);
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
            return (T)Deserialize(xml, typeof(T));
        }

        private object ReadXml(XElement currentNode, TypeDefinition type, object instance = null)
        {
            if (type.IsPrimitive)
            {
                return PrimitiveValueTools.GetPrimitiveValue(currentNode.Value, type, currentNode.Name.LocalName);
            }
            if (type.IsDictionary)
            {
                return ReadXmlDictionary(currentNode, type);
            }

            if (type.IsArray || type.IsEnumerable)
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
            var currentObject = instance ?? currentNodeDef.ObjectActivator();

            if (configuration != null)
            {
                if (configuration.IsObjectReference)
                {
                    string refId = currentNode.Attribute(Ref)?.Value;
                    if (!string.IsNullOrEmpty(refId))
                    {
                        var key = currentNodeDef.FullName + Underscore + refId;
                        if (_referencesObjects.ContainsKey(key))
                        {
                            return _referencesObjects[key];
                        }
                        _referencesObjects.Add(key, currentObject);
                    }
                    string objectId = currentNode.Attribute(Id)?.Value;
                    if (!string.IsNullOrEmpty(objectId))
                    {
                        var key = currentNodeDef.FullName + Underscore + objectId;
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
                var propertyInfo = type.GetProperty(localName);
                if (propertyInfo == null)
                {
                    throw new InvalidOperationException("Missing property " + currentNode.Name.LocalName + "\\" + localName);
                }
                var propertyDef = propertyInfo.TypeDefinition;
                if (xElement.HasAttributes && xElement.Attribute(Type) != null)
                {
                    // If type of property is saved in xml, we need check type of object actual assigned to property. There may be a base type. 
                    Type targetType = TypeDefinitionCache.GetType(xElement.Attribute(Type).Value);
                    var targetTypeDef = TypeDefinitionCache.GetDefinition(targetType);
                    var obj = propertyInfo.GetValue(currentObject);
                    if ((obj == null || obj.GetType() != targetType) && targetTypeDef.ObjectActivator != null)
                    {
                        obj = targetTypeDef.ObjectActivator();
                    }
                    var obj2 = ReadXml(xElement, targetTypeDef, obj);
                    propertyInfo.SetValue(currentObject, obj2);
                }
                else if (propertyDef.IsObjectToSerialize || propertyDef.IsArray || propertyDef.IsEnumerable || propertyDef.IsDictionary)
                {
                    //If xml does not contain type but we known that it is object
                    var obj = propertyInfo.GetValue(currentObject);
                    var obj2 = ReadXml(xElement, propertyDef, obj);
                    if ( obj == null && obj2 != null )
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

                    object primitive = PrimitiveValueTools.GetPrimitiveValue(value, propertyDef, xElement.Name.LocalName);
                    propertyInfo.SetValue(currentObject, primitive);
                }
            }
            return currentObject;
        }

        private object ReadXmlDictionary(XElement currentNode, TypeDefinition type, object instance = null)
        {
            int arrayCount = currentNode.Elements().Count();
            var elements = currentNode.Elements().ToArray();

            var definition = GetElementTypeDefinition(currentNode) ?? type;
            object dict = instance ?? definition.ObjectActivator();

            for (int i = 0; i < arrayCount; i++)
            {
                var element = elements[i];
                
                var key = element.Element(Key);
                var value = element.Element(Value);

                var keyDef = GetElementTypeDefinition(key, type.GenericArguments[0]);
                var valuDef = GetElementTypeDefinition(value, type.GenericArguments[1]);

                type.MethodAddToDictionary(dict, ReadXml(key, keyDef), ReadXml(value, valuDef));
            }
            return dict;
        }

        private object ReadXmlArray(XElement currentNode, TypeDefinition type, object instance = null)
        {
            var elements = currentNode.Elements().ToArray();
            int arrayCount = elements.Length;
            object list = null;
            Array array = null;
            if (type.IsArray)
            {
                array =  instance as Array ?? Array.CreateInstance(type.Type.GetElementType(), arrayCount);
            }
            else
            {
                var definition = GetElementTypeDefinition(currentNode) ?? type;
                list = instance ?? definition.ObjectActivator();
            }

            var elementType = ElementTypeLocator.Default.Locate( type.Type );
            for (int i = 0; i < arrayCount; i++)
            {
                var element = elements[i];
                var definition = GetElementTypeDefinition(element, elementType);

                var xml = ReadXml(element, definition);
                if (type.IsArray)
                {
                    array?.SetValue(xml, i);
                }
                else
                {
                    type.MethodAddToCollection(list, xml);
                }
            }
            if (type.IsArray)
            {
                return array;
            }
            return list;
        }

        private TypeDefinition GetElementTypeDefinition(XElement element, Type defuaultType = null)
        {
            var typeAttribute = element.Attribute(Type);
            if (typeAttribute != null)
            {
                return TypeDefinitionCache.GetDefinition(TypeDefinitionCache.GetType(typeAttribute.Value));
            }
            return defuaultType == null ? null : TypeDefinitionCache.GetDefinition(defuaultType);
        }

        private void WriteXmlPrimitive(object o, XmlWriter xw, TypeDefinition def, string name = null, bool toEncrypt = false, string valueType = null)
        {
            xw.WriteStartElement(name ?? def.PrimitiveName);
            if (!string.IsNullOrEmpty(valueType))
            {
                xw.WriteAttributeString(Type, valueType);
            }
            var value = PrimitiveValueTools.SetPrimitiveValue(o, def.Type);
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

        private void WriteXml(XmlWriter writer, object o, TypeDefinition type, string name = null, bool writeReservedObject = false, bool forceSaveType = false)
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
            if (type.IsArray || type.IsEnumerable)
            {
                WriteXmlArray(o, writer, type, name, forceSaveType);
                return;
            }
            writer.WriteStartElement(name ?? type.Name);
            writer.WriteAttributeString(Type, type.FullName);
            
            // Get configuration for type
            var configuration = GetConfiguration(type.Type);

            if (configuration != null)
            {
                if (configuration.IsObjectReference)
                {
                    var objectId = configuration.GetObjectId(o);
                    
                    var key = type.FullName + Underscore + objectId;
                    if (writeReservedObject && _reservedReferencesObjects.ContainsKey(key))
                    {
                        _reservedReferencesObjects.Remove(key);
                    }
                    else if (_referencesObjects.ContainsKey(key) || _reservedReferencesObjects.ContainsKey(key))
                    {
                        writer.WriteAttributeString(Ref, objectId);
                        writer.WriteEndElement();
                        return;
                    }
                    writer.WriteAttributeString(Id, objectId);
                    _referencesObjects.Add(key, o);
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

            var properties = type.Properties;
            foreach (var propertyInfo in properties)
            {
                var propertyValue = propertyInfo.GetValue(o);
                if (propertyValue == null)
                    continue;

                var defType = TypeDefinitionCache.GetDefinition(propertyValue.GetType());

                if (defType.IsObjectToSerialize || defType.IsArray || defType.IsEnumerable)
                {
                    WriteXml(writer, propertyValue, defType, propertyInfo.Name, forceSaveType: propertyInfo.MemberInfo.IsWritable() && propertyInfo.TypeDefinition.FullName != defType.FullName);
                }
                else if (defType.IsEnum)
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
                        WriteXmlPrimitive(propertyValue, writer, defType, propertyInfo.Name, toEncrypt, defType.Type.FullName);
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
