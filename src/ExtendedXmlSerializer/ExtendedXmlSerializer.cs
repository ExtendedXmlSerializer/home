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
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization
{
    /// <summary>
    /// Extended Xml Serializer
    /// </summary>
    public class ExtendedXmlSerializer : IExtendedXmlSerializer
    {
        private ISerializationToolsFactory _toolsFactory;

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
            var def = TypeDefinitionCache.GetDefinition(o.GetType());

            string xml;
            using (var ms = new MemoryStream())
            {
                using (XmlWriter xw = XmlWriter.Create(ms))
                {
                    if (def.IsPrimitive)
                    {
                        WriteXmlPrimitive(o, xw, def);
                    }
                    else
                    {
                        xw.WriteStartElement(def.Name);
                        WriteXml(xw, o, def);
                        xw.WriteEndElement();
                    }

                }
                ms.Position = 0;

                using (var sr = new StreamReader(ms))
                {
                    xml = sr.ReadToEnd();
                }
            }
            return xml;
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
            if (def.IsPrimitive)
            {
                if (doc.Root == null)
                {
                    throw new Exception("Missing root node.");
                }
                return PrimitiveValueTools.GetPrimitiveValue(doc.Root.Value, type, doc.Root.Name.LocalName);
            }
            return ReadXml(doc.Root, def);
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
            if (currentNode == null)
                return null;

            TypeDefinition currentNodeDef = null;
            // Retrieve type from XML (Property can be base type. In xml can be saved inherited object)
            var typeAttribute = currentNode.Attribute("type");
            if (typeAttribute != null)
            {
                var currentNodeType = TypeDefinitionCache.GetType(typeAttribute.Value);
                currentNodeDef = TypeDefinitionCache.GetDefinition(currentNodeType);
            }
            // If xml does not contain type get property type
            if (currentNodeDef == null)
            {
                currentNodeDef = type;
            }

            // Run migrator if exists
            var migrator = GetMigrationMap(currentNodeDef);
            if (migrator != null)
            {
                int currentNodeVer = 0;
                var serializeVersion = currentNode.Attribute("serializeVersion");
                if (serializeVersion != null)
                {
                    currentNodeVer = int.Parse(serializeVersion.Value);
                }
                migrator.Map(currentNodeDef.Type, currentNode, currentNodeVer);
            }
            // run custom serializer if exists
            var customSerializer = GetCustomSerializer(currentNodeDef);
            if (customSerializer != null)
            {
                return customSerializer.ReadObject(currentNode);
            }

            // Create new instance if not exists
            var currentObject = instance ?? currentNodeDef.ObjectActivator();
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
                var propertyDef = TypeDefinitionCache.GetDefinition(propertyInfo.Type);
                if (xElement.HasAttributes && xElement.Attribute("type") != null)
                {
                    // If type of property is saved in xml, we need check type of object actual assigned to property. There may be a base type. 
                    Type targetType = TypeDefinitionCache.GetType(xElement.Attribute("type").Value);
                    if (targetType == null)
                        throw new Exception($"Can not find the type {xElement.Attribute("type").Value}");
                    var targetTypeDef = TypeDefinitionCache.GetDefinition(targetType);
                    var obj = propertyInfo.Getter(currentObject);
                    if (obj == null || obj.GetType() != targetType)
                    {
                        obj = targetTypeDef.ObjectActivator();
                    }
                    var obj2 = ReadXml(xElement, targetTypeDef, obj);
                    propertyInfo.Setter(currentObject, obj2);
                }
                else if (propertyDef.IsObjectToSerialize)
                {
                    //If xml does not contain type but we known that it is object
                    var obj = propertyInfo.Getter(currentObject);
                    if (obj == null)
                    {
                        obj = propertyDef.ObjectActivator();
                    }
                    var obj2 = ReadXml(xElement, propertyDef, obj);
                    propertyInfo.Setter(currentObject, obj2);
                }
                else if (propertyDef.IsGenericList)
                {
                    IList obj = propertyInfo.Getter(currentObject) as IList ??
                                (IList)propertyDef.ObjectActivator();
                    Type itemType = propertyDef.GenericArguments[0];
                    var itemTypeDef = TypeDefinitionCache.GetDefinition(itemType);
                    foreach (XElement item in xElement.Elements())
                    {
                        if (itemTypeDef.IsReadAsPrimitive)
                        {
                            object primitive = PrimitiveValueTools.GetPrimitiveValue(item.Value, itemType, xElement.Name.LocalName);
                            obj.Add(primitive);
                        }
                        else
                        {
                            obj.Add(ReadXml(item, itemTypeDef));
                        }
                    }
                    propertyInfo.Setter(currentObject, obj);
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    object primitive = PrimitiveValueTools.GetPrimitiveValue(value, propertyInfo.Type, xElement.Name.LocalName);
                    propertyInfo.Setter(currentObject, primitive);
                }
            }
            return currentObject;
        }

        private static void WriteXmlPrimitive(object o, XmlWriter xw, TypeDefinition def)
        {
            xw.WriteStartElement(def.PrimitiveName);
            xw.WriteString(PrimitiveValueTools.SetPrimitiveValue(o, def.Type));
            xw.WriteEndElement();
        }

        private void WriteXml(XmlWriter writer, object o, TypeDefinition type)
        {
            writer.WriteAttributeString("type", type.FullName);

            var migraor = GetMigrationMap(type);
            if (migraor != null && migraor.Version > 0)
            {
                writer.WriteAttributeString("serializeVersion",
                    migraor.Version.ToString(CultureInfo.InvariantCulture));
            }

            var customSerializer = GetCustomSerializer(type);
            if (customSerializer != null)
            {
                customSerializer.WriteObject(writer, o);
                return;
            }

            var properties = type.Properties;
            foreach (var propertyInfo in properties)
            {
                var propertyValue = propertyInfo.Getter(o);
                if (propertyValue == null)
                    continue;

                var defType = TypeDefinitionCache.GetDefinition(propertyValue.GetType());

                if (defType.IsObjectToSerialize)
                {
                    writer.WriteStartElement(propertyInfo.Name);
                    WriteXml(writer, propertyValue, defType);
                    writer.WriteEndElement();
                }
                else if (defType.IsEnum)
                {
                    writer.WriteStartElement(propertyInfo.Name);
                    writer.WriteString(propertyValue.ToString());
                    writer.WriteEndElement();
                }
                else if (defType.IsGenericList)
                {
                    writer.WriteStartElement(propertyInfo.Name);
                    var lista = propertyValue as IList;
                    if (lista != null && lista.Count > 0)
                    {
                        foreach (var element in lista)
                        {
                            var defElement = TypeDefinitionCache.GetDefinition(element.GetType());
                            if (defElement.IsPrimitive)
                            {
                                WriteXmlPrimitive(element, writer, defElement);
                            }
                            else
                            {
                                writer.WriteStartElement(defElement.Name);
                                WriteXml(writer, element, defElement);
                                writer.WriteEndElement();
                            }
                        }
                    }
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement(propertyInfo.Name);
                    writer.WriteString(PrimitiveValueTools.SetPrimitiveValue(propertyValue, defType.Type));
                    writer.WriteEndElement();
                }
            }
        }

        private ICustomSerializator GetCustomSerializer(TypeDefinition type)
        {
            return _toolsFactory?.GetCustomSerializer(type.Type);
        }

        private IMigrationMap GetMigrationMap(TypeDefinition type)
        {
            return _toolsFactory?.GetMigrationMap(type.Type);
        }
    }
}
