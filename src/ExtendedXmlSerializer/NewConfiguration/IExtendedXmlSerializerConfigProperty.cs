using System;
using System.Linq.Expressions;

namespace ExtendedXmlSerialization.NewConfiguration
{
    public interface IExtendedXmlSerializerConfigProperty<T, TProperty>
    {
        IExtendedXmlSerializerConfigProperty<T, TOtherProperty> Property<TOtherProperty>(Expression<Func<T, TOtherProperty>> property);
        IExtendedXmlSerializerConfigProperty<T, TProperty> ObjectReference();
        IExtendedXmlSerializerConfigProperty<T, TProperty> AsAttribute();
        IExtendedXmlSerializerConfigProperty<T, TProperty> Encrypt();
        IExtendedXmlSerializerConfigProperty<T, TProperty> Name(string name);
        IExtendedXmlSerializerConfigProperty<T, TProperty> Order(int order);
    }

    internal interface IExtendedXmlSerializerConfigProperty
    {
        bool IsObjectReference { get; set; }
        bool IsAttribute { get; set; }
        bool IsEncrypt { get; set; }
        string ChangedName { get; set; }
        int ChangedOrder { get; set; }
    }
}