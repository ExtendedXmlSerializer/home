using System;
using System.Linq.Expressions;

namespace ExtendedXmlSerialization.Configuration
{
    public interface IExtendedXmlPropertyConfiguration<T, TProperty>
    {
        IExtendedXmlPropertyConfiguration<T, TOtherProperty> Property<TOtherProperty>(Expression<Func<T, TOtherProperty>> property);
        IExtendedXmlPropertyConfiguration<T, TProperty> ObjectReference();
        IExtendedXmlPropertyConfiguration<T, TProperty> AsAttribute();
        IExtendedXmlPropertyConfiguration<T, TProperty> Encrypt();
        IExtendedXmlPropertyConfiguration<T, TProperty> Name(string name);
        IExtendedXmlPropertyConfiguration<T, TProperty> Order(int order);
    }

    internal interface IExtendedXmlPropertyConfiguration
    {
        bool IsObjectReference { get; set; }
        bool IsAttribute { get; set; }
        bool IsEncrypt { get; set; }
        string ChangedName { get; set; }
        int ChangedOrder { get; set; }
    }
}