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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Model
{
    internal class TypeDefinition : ITypeDefinition
    {
        readonly private static IDictionary<TypeCode, string> Codes = new Dictionary<TypeCode, string>
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
                                                                      };

        readonly private static IDictionary<Type, string> Other = new Dictionary<Type, string>
                                                                  {
                                                                      {typeof(Guid), "guid"},
                                                                      {typeof(TimeSpan), "TimeSpan"},
                                                                  };

        readonly private static Type TypeObject = typeof(object);

        readonly private Lazy<ImmutableArray<IMemberDefinition>> _members =
            new Lazy<ImmutableArray<IMemberDefinition>>(Empty);

        public TypeDefinition(Type type)
        {
            Type = Nullable.GetUnderlyingType(type) ?? type;
            TypeCode = Type.GetTypeCode(Type);
            var typeInfo = Type.GetTypeInfo();
            string name;
            IsPrimitive = Codes.TryGetValue(TypeCode, out name) || Other.TryGetValue(Type, out name) || typeInfo.IsEnum;


            var isGenericType = typeInfo.IsGenericType;

            Name = typeInfo.GetCustomAttribute<XmlRootAttribute>()?.ElementName ?? (IsPrimitive ? name : Type.Name);

            if (isGenericType)
            {
                Type[] types = Type.GetGenericArguments();
                Name = Name.Replace($"`{types.Length.ToString()}",
                                    $"Of{string.Join(string.Empty, types.Select(p => p.Name))}");
            }

            if (!IsPrimitive)
            {
                var elementType = ElementTypeLocator.Default.Locate(Type);
                IsEnumerable = elementType != null;
                if (IsEnumerable)
                {
                    var isArray = typeInfo.IsArray;
                    IsDictionary = typeof(IDictionary).IsAssignableFrom(Type) ||
                                   typeof(IDictionary<,>).IsAssignableFromGeneric(Type);


                    GenericArguments = isGenericType
                        ? Type.GetGenericArguments().ToImmutableArray()
                        : ImmutableArray.Create(elementType);

                    Name = isArray || isGenericType
                        ? $"ArrayOf{string.Join(string.Empty, GenericArguments.Select(p => p.Name))}"
                        : Type.Name;

                    if (!isArray)
                    {
                        MethodInfo add = AddMethodLocator.Default.Locate(type, elementType);
                        MethodAddToCollection = add != null
                            ? ObjectAccessors.CreateMethodAddCollection(Type, elementType, add)
                            : null;
                    }
                }

                var members = Type != TypeObject &&
                              //not generic or generic but not List<> and Set<>
                              (!isGenericType || !IsEnumerable);
                if (members)
                {
                    _members =
                        new Lazy<ImmutableArray<IMemberDefinition>>(CreateMembers);
                }

                ObjectActivator = ObjectAccessors.CreateObjectActivator(Type);
            }
        }

        public ObjectAccessors.AddItemToCollection MethodAddToCollection { get; }
        
        private static ImmutableArray<IMemberDefinition> Empty() => ImmutableArray<IMemberDefinition>.Empty;

        private ImmutableArray<IMemberDefinition> CreateMembers()
            => YieldMembers().OrderBy(x => x, MemberComparer.Default).ToImmutableArray();

        private IEnumerable<IMemberDefinition> YieldMembers()
        {
            foreach (PropertyInfo propertyInfo in Type.GetProperties())
            {
                var elementType = ElementTypeLocator.Default.Locate(propertyInfo.PropertyType);
                var getMethod = propertyInfo.GetGetMethod(true);
                if (propertyInfo.CanRead && !getMethod.IsStatic && getMethod.IsPublic &&
                    (propertyInfo.CanWrite || elementType != null) &&
                    !(!propertyInfo.GetSetMethod(true)?.IsPublic ?? false) &&
                    propertyInfo.GetIndexParameters().Length <= 0 &&
                    !propertyInfo.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    var xmlElement =
                        propertyInfo.GetCustomAttributes(false).FirstOrDefault(a => a is XmlElementAttribute) as
                            XmlElementAttribute;
                    var definition = new MemberDefinition(propertyInfo,
                                                          xmlElement?.ElementName.NullIfEmpty() ?? propertyInfo.Name)
                                     {
                                         Order = xmlElement?.Order ?? -1
                                     };
                    yield return definition;
                }
            }

            foreach (FieldInfo field in Type.GetFields())
            {
                if ((field.IsInitOnly ? !field.IsLiteral : !field.IsStatic) &&
                    !field.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    var xmlElement = field.GetCustomAttribute<XmlElementAttribute>(false);
                    var definition = new MemberDefinition(field, xmlElement?.ElementName.NullIfEmpty() ?? field.Name)
                                     {
                                         Order = xmlElement?.Order ?? -1
                                     };
                    yield return definition;
                }
            }
        }

        class MemberComparer : IComparer<IMemberDefinition>
        {
            public static MemberComparer Default { get; } = new MemberComparer();
            MemberComparer() {}

            public int Compare(IMemberDefinition x, IMemberDefinition y)
            {
                if (x.Order == -1 || y.Order == -1)
                {
                    if (x.Order > -1)
                    {
                        return -1;
                    }
                    if (y.Order > -1)
                    {
                        return 1;
                    }
                    return x.Metadata.MetadataToken.CompareTo(y.Metadata.MetadataToken);
                }

                return x.Order.CompareTo(y.Order);
            }
        }

        public bool IsPrimitive { get; }

        public void Add(object item, object value) => MethodAddToCollection?.Invoke(item, value);
        
        public bool CanActivate => ObjectActivator != null;

        public object Activate() => ObjectActivator();

        public bool IsEnumerable { get; }
        public bool IsDictionary { get; }
        public ImmutableArray<Type> GenericArguments { get; }

        public ImmutableArray<IMemberDefinition> Members => _members.Value;

        public Type Type { get; }
        public string Name { get; }

        public TypeCode TypeCode { get; }

        public IMemberDefinition this[string memberName]
        {
            get
            {
                var members = Members;
                for (int i = 0; i < members.Length; i++)
                {
                    var item = members[i];
                    if (item.Name == memberName)
                    {
                        return item;
                    }
                }
                return null;
            }
        }

        public ObjectAccessors.ObjectActivator ObjectActivator { get; }
    }
}