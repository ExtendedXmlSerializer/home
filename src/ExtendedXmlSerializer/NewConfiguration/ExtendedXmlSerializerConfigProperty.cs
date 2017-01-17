using System;
using System.Linq.Expressions;

namespace ExtendedXmlSerialization.NewConfiguration
{
    internal class ExtendedXmlSerializerConfigProperty<T, TProperty> :
        IExtendedXmlSerializerConfigProperty<T, TProperty>, IExtendedXmlSerializerConfigProperty
    {
        public ExtendedXmlSerializerConfigType<T> ConfigType { get; set; }
        public Expression<Func<T, TProperty>> PropertyExpression { get; set; }
    
        public bool IsObjectReference { get; set; }
        public bool IsAttribute { get; set; }
        public bool IsEncrypt { get; set; }
        public string ChangedName { get; set; }
        public int ChangedOrder { get; set; }
        public IExtendedXmlSerializerConfigProperty<T, TOtherProperty> Property<TOtherProperty>(Expression<Func<T, TOtherProperty>> property)
        {
            return ConfigType.Property(property);
        }

        public IExtendedXmlSerializerConfigProperty<T, TProperty> ObjectReference()
        {
            IsObjectReference = true;
            ConfigType.IsObjectReference = true;
            ConfigType.GetObjectId = p => PropertyExpression.Compile()((T) p).ToString();
            return this;
        }

        public IExtendedXmlSerializerConfigProperty<T, TProperty> AsAttribute()
        {
            IsAttribute = true;
            return this;
        }

        public IExtendedXmlSerializerConfigProperty<T, TProperty> Encrypt()
        {
            IsEncrypt = true;
            return this;
        }

        public IExtendedXmlSerializerConfigProperty<T, TProperty> Name(string name)
        {
            ChangedName = name;
            return this;
        }

        public IExtendedXmlSerializerConfigProperty<T, TProperty> Order(int order)
        {
            ChangedOrder = order;
            return this;
        }
    }
}