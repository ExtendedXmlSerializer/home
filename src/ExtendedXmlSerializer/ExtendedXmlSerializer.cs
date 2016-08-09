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
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization
{
    /// <summary>
    /// Extended Xml Serializer
    /// </summary>
    public static class ExtendedXmlSerializer
    {
        /// <summary>
        /// Serializes the specified <see cref="T:System.Object" /> and returns xml document in string
        /// </summary>
        /// <param name="o">The <see cref="T:System.Object" /> to serialize. </param>
        /// <returns>xml document in string</returns>
        public static string Serialize(object o)
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
        public static object Deserialize(string xml, Type type)
        {
            var def = TypeDefinitionCache.GetDefinition(type);
            XDocument doc = XDocument.Parse(xml);
            if (def.IsPrimitive)
            {
                if (doc.Root == null)
                {
                    throw new Exception("Missing root node.");
                }
                return GetPrimitiveValue(doc.Root.Value, type, doc.Root.Name.LocalName);
            }
            return ReadXml(doc.Root, def);
        }

        /// <summary>
        /// Deserializes the XML document
        /// </summary>
        /// <typeparam name="T">The type of returned object</typeparam>
        /// <param name="xml">The XML document</param>
        /// <returns>deserialized object</returns>
        public static T Deserialize<T>(string xml)
        {
            return (T)Deserialize(xml, typeof(T));
        }

        /// <summary>
        /// Deep clone object
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="obj">The object to clone</param>
        /// <returns>Cloned object</returns>
        public static T DeepCloneObject<T>(this T obj) where T: class
        {
            var xml = Serialize(obj);
            return Deserialize<T>(xml);
        }

        private static object ReadXml(XElement currentNode, TypeDefinition type, object instance = null)
        {
            if (currentNode == null)
                return null;

            TypeDefinition currentNodeDef = null;
            // Retrieve type from XML (Property can be base type. In xml can be saved inherited object)
            var typeAttribute = currentNode.Attribute("type");
            if (typeAttribute != null)
            {
                var currentNodeType = GetType(typeAttribute.Value);
                currentNodeDef = TypeDefinitionCache.GetDefinition(currentNodeType);            
            }
            // If xml does not contain type get property type
            if (currentNodeDef == null)
            {
                currentNodeDef = type;
            }

            // Run migrator if exists
            var migrator = currentNodeDef.MigrationMap;
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
            var customSerializer = currentNodeDef.SerializableModel;
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
                    throw new InvalidOperationException("Missing property " + currentNode.Name.LocalName + "\\"+localName);
                }
                var propertyDef = TypeDefinitionCache.GetDefinition(propertyInfo.Type);
                if (xElement.HasAttributes && xElement.Attribute("type") != null)
                {
                    // If type of property is saved in xml, we need check type of object actual assigned to property. There may be a base type. 
                    Type targetType = GetType(xElement.Attribute("type").Value);
                    if(targetType == null)
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
                            object primitive = GetPrimitiveValue(item.Value, itemType, xElement.Name.LocalName);
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
                    object primitive = GetPrimitiveValue(value, propertyInfo.Type, xElement.Name.LocalName);
                    propertyInfo.Setter(currentObject, primitive);
                }
            }
            return currentObject;
        }

        private static void WriteXmlPrimitive(object o, XmlWriter xw, TypeDefinition def)
        {
            xw.WriteStartElement(def.PrimitiveName);
            xw.WriteString(SetPrimitiveValue(o, def.Type));
            xw.WriteEndElement();
        }

        private static void WriteXml(XmlWriter writer, object o, TypeDefinition type)
        {
            writer.WriteAttributeString("type", type.FullName);
    
            var migraor = type.MigrationMap;
            if (migraor != null && migraor.Version > 0)
            {
                writer.WriteAttributeString("serializeVersion",
                    migraor.Version.ToString(CultureInfo.InvariantCulture));
            }
            
            var customSerializer = type.SerializableModel;
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
                    writer.WriteString(SetPrimitiveValue(propertyValue, defType.Type));
                    writer.WriteEndElement();
                }
            }
        }

        private static string SetPrimitiveValue(object value, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return value.ToString();
                case TypeCode.Char:
                    return XmlConvert.ToString((UInt16)(char) value);
                case TypeCode.SByte:
                    return XmlConvert.ToString((sbyte) value);
                case TypeCode.Byte:
                    return XmlConvert.ToString((byte) value);
                case TypeCode.Int16:
                    return XmlConvert.ToString((short) value);
                case TypeCode.UInt16:
                    return XmlConvert.ToString((ushort) value);
                case TypeCode.Int32:
                    return XmlConvert.ToString((int) value);
                case TypeCode.UInt32:
                    return XmlConvert.ToString((uint) value);
                case TypeCode.Int64:
                    return XmlConvert.ToString((long) value);
                case TypeCode.UInt64:
                    return XmlConvert.ToString((ulong) value);
                case TypeCode.Single:
                    return XmlConvert.ToString((float) value);
                case TypeCode.Double:
                    return XmlConvert.ToString((double) value);
                case TypeCode.Decimal:
                    return XmlConvert.ToString((decimal) value);
                case TypeCode.DateTime:
                    return XmlConvert.ToString((DateTime) value, XmlDateTimeSerializationMode.RoundtripKind);
                case TypeCode.String:
                    return (string) value;
                default:
                    if (type == typeof(Guid))
                    {
                        return XmlConvert.ToString((Guid) value);
                    }
                    if (type == typeof(TimeSpan))
                    {
                        return XmlConvert.ToString((TimeSpan) value);
                    }
                    return value.ToString();
            }
        }

        private static Type GetType(string typeName)
        {
            return TypeDefinitionCache.GetType(typeName);
        }

        private static string DecimalSeparator(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Replace(",", ".");
        }

        private static object GetPrimitiveValue(string value, Type type, string nodeName)
        {
            try
            {
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
                {
                    type = type.GetGenericArguments()[0];
                }
                if (type.GetTypeInfo().IsEnum)
                {
                    return Enum.Parse(type, value);
                }
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        return Convert.ToBoolean(value);
                    case TypeCode.Char:
                        return (char)XmlConvert.ToUInt16(value);
                    case TypeCode.SByte:
                        return XmlConvert.ToSByte(value);
                    case TypeCode.Byte:
                        return XmlConvert.ToByte(value);
                    case TypeCode.Int16:
                        return XmlConvert.ToInt16(value);
                    case TypeCode.UInt16:
                        return XmlConvert.ToUInt16(value);
                    case TypeCode.Int32:
                        return XmlConvert.ToInt32(value);
                    case TypeCode.UInt32:
                        return XmlConvert.ToUInt32(value);
                    case TypeCode.Int64:
                        return XmlConvert.ToInt64(value);
                    case TypeCode.UInt64:
                        return XmlConvert.ToUInt64(value);
                    case TypeCode.Single:
                        return XmlConvert.ToSingle(DecimalSeparator(value));
                    case TypeCode.Double:
                        return XmlConvert.ToDouble(DecimalSeparator(value));
                    case TypeCode.Decimal:
                        return XmlConvert.ToDecimal(DecimalSeparator(value));
                    case TypeCode.DateTime:
                        return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
                    case TypeCode.String:
                        return value;
                    default:
                        if (type == typeof(Guid))
                        {
                            return XmlConvert.ToGuid(value);
                        }
                        if (type == typeof(TimeSpan))
                        {
                            return XmlConvert.ToTimeSpan(value);
                        }
                        throw new NotSupportedException("Unknown primitive type " + type.Name + " - value: " + value);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unsuccessful conversion node {nodeName} for type {type.Name} - value {value}", ex);
            }
        }
    }
}