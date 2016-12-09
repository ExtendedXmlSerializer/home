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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization.Cache
{
    internal class TypeDefinition
    {
        readonly static IDictionary<TypeCode, string> Codes = new Dictionary<TypeCode, string>
                                                              {
                                                                  {TypeCode.Boolean, "boolean"},
                                                                  {TypeCode.Char, "char"},
                                                                  {TypeCode.SByte, "byte"},
                                                                  {TypeCode.Byte, "unsignedByte"},
                                                                  {TypeCode.Int16, "short"},
                                                                  {TypeCode.UInt16, "unsignedShort"},
                                                                  {TypeCode.Int32, "int"},
                                                                  {TypeCode.UInt32, "unsignedInt"},
                                                                  {TypeCode.Int64, "long"},
                                                                  {TypeCode.UInt64, "unsignedLong"},
                                                                  {TypeCode.Single, "float"},
                                                                  {TypeCode.Double, "double"},
                                                                  {TypeCode.Decimal, "decimal"},
                                                                  {TypeCode.DateTime, "dateTime"},
                                                                  {TypeCode.String, "string"},
                                                              }.ToImmutableDictionary();

        readonly private static IDictionary<Type, string> Other = new Dictionary<Type, string>
                                                                  {
                                                                      {typeof(Guid), "guid"},
                                                                      {typeof(TimeSpan), "TimeSpan"},
                                                                  }.ToImmutableDictionary();

        readonly Lazy<IEnumerable<PropertieDefinition>> _properties;
        readonly private static Type TypeObject = typeof(object);

        public TypeDefinition(Type type)
        {
            Type = Nullable.GetUnderlyingType(type) ?? type;
            TypeCode = Type.GetTypeCode(Type);
            string name;
            IsPrimitive = Codes.TryGetValue(TypeCode, out name) || Other.TryGetValue(Type, out name);
            Name = IsPrimitive ? name : Type.Name;

            var typeInfo = Type.GetTypeInfo();
            var isGenericType = typeInfo.IsGenericType;
            
            if (isGenericType)
            {
                Type[] types = Type.GetGenericArguments();
                Name = Name.Replace($"`{types.Length}", $"Of{string.Join(string.Empty, types.Select(p => p.Name))}");
            }
            
            FullName = Type.FullName;

            IsEnum = typeInfo.IsEnum;
            IsArray = typeInfo.IsArray;
            IsEnumerable = !IsPrimitive && typeof(IEnumerable).IsAssignableFrom(Type);

            if (IsEnumerable)
            {
                IsDictionary = typeof(IDictionary).IsAssignableFrom(Type) || typeof(IDictionary<,>).IsAssignableFromGeneric(Type);

                var elementType = ElementTypeLocator.Default.Locate(Type);
                if (isGenericType)
                {
                    GenericArguments = Type.GetGenericArguments();
                }
                else if ( elementType != null )
                {
                    GenericArguments = new[] { elementType };
                }

                Name = IsArray || isGenericType ? $"ArrayOf{string.Join(string.Empty, GenericArguments.Select(p => p.Name))}" : Type.Name;

                if (IsDictionary)
                {
                    MethodAddToDictionary = ObjectAccessors.CreateMethodAddToDictionary(Type);
                }
                else if (elementType != null && !IsArray)
                {
					MethodInfo add = AddMethodLocator.Default.Locate(type, elementType);
					MethodAddToCollection = add != null ? ObjectAccessors.CreateMethodAddCollection(Type, elementType, add) : null;
                }
            }

            XmlRootAttribute attribute = typeInfo.GetCustomAttribute<XmlRootAttribute>();
            if (attribute != null)
            {
                Name = attribute.ElementName;
            }

            IsObjectToSerialize = // !typeInfo.IsPrimitive && !typeInfo.IsValueType &&
                !IsPrimitive &&
                !typeInfo.IsEnum && Type != TypeObject &&
                //not generic or generic but not List<> and Set<>
                (!isGenericType || !IsEnumerable);
            _properties = new Lazy<IEnumerable<PropertieDefinition>>( GetPropertieToSerialze );
            
            ObjectActivator = ObjectAccessors.CreateObjectActivator(Type, IsPrimitive);
        }

        public ObjectAccessors.AddItemToCollection MethodAddToCollection { get; set; }
        public ObjectAccessors.AddItemToDictionary MethodAddToDictionary { get; set; }

        private IEnumerable<PropertieDefinition> GetPropertieToSerialze()
        {
            var result = new List<PropertieDefinition>();
            if ( IsObjectToSerialize )
            {
                int order = -1;
            
                foreach (PropertyInfo propertyInfo in Type.GetProperties())
                {
                    var elementType = ElementTypeLocator.Default.Locate( propertyInfo.PropertyType );
                    var getMethod = propertyInfo.GetGetMethod(true);
                    if (!propertyInfo.CanRead || getMethod.IsStatic || !getMethod.IsPublic ||
                       !propertyInfo.CanWrite && elementType == null || ( !propertyInfo.GetSetMethod(true)?.IsPublic ?? false ) ||
                        propertyInfo.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    bool ignore = propertyInfo.GetCustomAttributes(false).Any(a => a is XmlIgnoreAttribute);
                    if (ignore)
                    {
                        continue;
                    }

                    var name = string.Empty;
                    order = -1;
                    var xmlElement = propertyInfo.GetCustomAttributes(false).FirstOrDefault(a => a is XmlElementAttribute) as XmlElementAttribute;
                    if (xmlElement != null)
                    {
                        name = xmlElement.ElementName;
                        order = xmlElement.Order;
                    }
                    var property = new PropertieDefinition(propertyInfo, name);
                    property.MetadataToken = propertyInfo.MetadataToken;
                    if (order != -1)
                    {
                        property.Order = order;
                    }
                    MemberNames.Default.Add(propertyInfo, property.Name);
                    result.Add(property);
                }

                var fields = Type.GetFields();
                foreach (FieldInfo field in fields)
                {
                
                    if (field.IsLiteral && !field.IsInitOnly)
                    {
                        continue;
                    }
                    if (field.IsInitOnly || field.IsStatic)
                    {
                        continue;
                    }
                    bool ignore = field.GetCustomAttributes(false).Any(a => a is XmlIgnoreAttribute);
                    if (ignore)
                    {
                        continue;
                    }
                    var name = string.Empty;
                    order = -1;
                    var xmlElement = field.GetCustomAttributes(false).FirstOrDefault(a => a is XmlElementAttribute) as XmlElementAttribute;
                    if (xmlElement != null)
                    {
                        name = xmlElement.ElementName;
                        order = xmlElement.Order;
                    }

                    var property = new PropertieDefinition(field, name);
                    property.MetadataToken = field.MetadataToken;
                    if (order != -1)
                    {
                        property.Order = order;
                    }
                    MemberNames.Default.Add(field, property.Name);
                    result.Add(property);
                }
            
                result.Sort((p1, p2) =>
                    {
                        if (p1.Order == -1 || p2.Order == -1)
                        {
                            if (p1.Order > -1)
                            {
                                return -1;
                            }
                            if (p2.Order > -1)
                            {
                                return 1;
                            }
                            return p1.MetadataToken.CompareTo(p2.MetadataToken);
                        }

                        return p1.Order.CompareTo(p2.Order);
                    }
                );
            }
            return result;
        }

        public bool IsPrimitive { get; }
        public bool IsArray { get; }
        public bool IsEnumerable { get; }
        public bool IsDictionary { get; }
        public Type[] GenericArguments { get; }

        public IEnumerable<PropertieDefinition> Properties => _properties.Value;
        public Type Type { get; }
        public string Name { get; }
        public string FullName { get; private set; }
        public bool IsObjectToSerialize { get; }
        public bool IsEnum { get; }

        public TypeCode TypeCode { get; }
        public ObjectAccessors.ObjectActivator ObjectActivator { get; private set; }

        public PropertieDefinition GetProperty(string name)
        {
            return Properties.FirstOrDefault(p => p.Name == name);
        }
    }
}
