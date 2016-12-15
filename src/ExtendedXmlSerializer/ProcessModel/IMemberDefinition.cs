using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization.ProcessModel
{
    public interface IMemberDefinition
    {
        bool IsWritable { get; }
        MemberInfo Metadata { get; }
        int MetadataToken { get; set; }
        string Name { get; }
        int Order { get; set; }
        ITypeDefinition MemberType { get; }

        object GetValue(object obj);
        void SetValue(object obj, object value);
    }

    public interface ITypeDefinition
    {
        string FullName { get; }
        ImmutableArray<Type> GenericArguments { get; }
        bool IsArray { get; }
        bool IsDictionary { get; }
        bool IsEnum { get; }
        bool IsEnumerable { get; }
        bool IsObjectToSerialize { get; }
        bool IsPrimitive { get; }

        void Add(object item, object value);

        void Add(object item, object key, object value);

        object Activate();

        string Name { get; }
        IImmutableList<IMemberDefinition> Members { get; }
        Type Type { get; }
        TypeCode TypeCode { get; }

        IMemberDefinition this[string memberName] { get; }
    }
}