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
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace ExtendedXmlSerialization.Cache
{
    internal class TypeDefinition
    {
        public TypeDefinition(Type type)
        {
            Type = type;
            Name = type.Name;

            var typeInfo= type.GetTypeInfo();
            if (typeInfo.IsGenericType)
            {
                Type[] types = type.GetGenericArguments();
                Name = Name.Replace("`" + types.Length, "Of" + string.Join("", types.Select(p=>p.Name)));
                }
            FullName = type.FullName;

            IsPrimitive = IsTypPrimitive(type);
            if (IsPrimitive)
            {
                PrimitiveName = GetPrimitiveName(type);
            }

            IsReadAsPrimitive = typeInfo.IsPrimitive || typeInfo.IsValueType || type == typeof(string) ||
                                (typeInfo.IsGenericType &&
                                 type.GetGenericTypeDefinition() == typeof(Nullable<>));

            IsObjectToSerialize = !typeInfo.IsPrimitive && !typeInfo.IsValueType &&
                                  !typeInfo.IsEnum && !(type == typeof(string)) &&
                                  //not generic or generic but not List<>
                                  (!typeInfo.IsGenericType || (typeInfo.IsGenericType && type.GetGenericTypeDefinition() != typeof(List<>)));
            IsEnum = typeInfo.IsEnum;

            IsGenericList = typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

            if (IsGenericList)
            {
                GenericArguments = type.GetGenericArguments();
            }
            Properties = GetPropertieToSerialze(type);

            ObjectActivator = ObjectAccessors.CreateObjectActivator(type);
        }

        public Type[] GenericArguments { get; private set; }
        
        private static List<PropertieDefinition> GetPropertieToSerialze(Type type)
        {
            var result = new List<PropertieDefinition>();
            var properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (!propertyInfo.CanWrite || !propertyInfo.GetSetMethod(true).IsPublic || propertyInfo.GetIndexParameters().Length > 0)
                    continue;

                bool ignore = propertyInfo.GetCustomAttributes(false).Any(a => a is XmlIgnoreAttribute);
                if (ignore) continue;

                result.Add(new PropertieDefinition(type, propertyInfo));
            }
            return result;
        }
        public bool IsPrimitive { get; private set; }
        public bool IsReadAsPrimitive { get; private set; }
        public List<PropertieDefinition> Properties { get; private set; }
        public Type Type { get; private set; }
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public bool IsObjectToSerialize { get; private set; }
        public bool IsEnum { get; private set; }
        public bool IsGenericList { get; private set; }
        public string PrimitiveName { get; private set; }
        public ObjectAccessors.ObjectActivator ObjectActivator { get; private set; }
        
        public PropertieDefinition GetProperty(string name)
        {
            return Properties.FirstOrDefault(p => p.Name == name);
        }

        private static bool IsTypPrimitive(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    return false;
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:                  
                    return true;
                default:
                    return false;
            }
        }
        private static string GetPrimitiveName(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return "boolean";
                case TypeCode.Char:
                    return "char";
                case TypeCode.SByte:
                    return "byte";
                case TypeCode.Byte:
                    return "unsignedByte";
                case TypeCode.Int16:
                    return "short";
                case TypeCode.UInt16:
                    return "unsignedShort";
                case TypeCode.Int32:
                    return "int";
                case TypeCode.UInt32:
                    return "unsignedInt";
                case TypeCode.Int64:
                    return "long";
                case TypeCode.UInt64:
                    return "unsignedLong";
                case TypeCode.Single:
                    return "float";
                case TypeCode.Double:
                    return "double";
                case TypeCode.Decimal:
                    return "decimal";
                case TypeCode.DateTime:
                    return "dateTime";
                case TypeCode.String:
                    return "string";
                default:
                    throw new InvalidOperationException("Unknown primitive type " + type.FullName);
            }
        }


    }
}
