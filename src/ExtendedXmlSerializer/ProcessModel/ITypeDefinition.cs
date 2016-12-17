using System;
using System.Collections.Immutable;

namespace ExtendedXmlSerialization.ProcessModel
{
    public interface IDefinition
    {
        string Name { get; }
        Type Type { get; }
    }

    public interface ITypeDefinition : IDefinition
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

        
        IImmutableList<IMemberDefinition> Members { get; }
        
        TypeCode TypeCode { get; }

        IMemberDefinition this[string memberName] { get; }
    }
}