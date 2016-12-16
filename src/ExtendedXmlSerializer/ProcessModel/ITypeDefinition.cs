using System;
using System.Collections.Immutable;

namespace ExtendedXmlSerialization.ProcessModel
{
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

        object DefaultValue { get; }

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