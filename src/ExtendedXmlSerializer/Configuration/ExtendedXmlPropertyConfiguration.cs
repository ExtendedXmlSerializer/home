using System;
using System.Linq.Expressions;

namespace ExtendedXmlSerialization.Configuration
{
    internal class ExtendedXmlPropertyConfiguration<T, TProperty> :
        IExtendedXmlPropertyConfiguration<T, TProperty>, IExtendedXmlPropertyConfiguration
    {
        public ExtendedXmlTypeConfiguration<T> TypeConfiguration { get; set; }
        public Expression<Func<T, TProperty>> PropertyExpression { get; set; }
    
        public bool IsObjectReference { get; set; }
        public bool IsAttribute { get; set; }
        public bool IsEncrypt { get; set; }
        public string ChangedName { get; set; }
        public int ChangedOrder { get; set; }
        public IExtendedXmlPropertyConfiguration<T, TOtherProperty> Property<TOtherProperty>(Expression<Func<T, TOtherProperty>> property)
        {
            return TypeConfiguration.Property(property);
        }

        public IExtendedXmlPropertyConfiguration<T, TProperty> ObjectReference()
        {
            IsObjectReference = true;
            TypeConfiguration.IsObjectReference = true;
            TypeConfiguration.GetObjectId = p => PropertyExpression.Compile()((T) p).ToString();
            return this;
        }

        public IExtendedXmlPropertyConfiguration<T, TProperty> AsAttribute()
        {
            IsAttribute = true;
            return this;
        }

        public IExtendedXmlPropertyConfiguration<T, TProperty> Encrypt()
        {
            IsEncrypt = true;
            return this;
        }

        public IExtendedXmlPropertyConfiguration<T, TProperty> Name(string name)
        {
            ChangedName = name;
            return this;
        }

        public IExtendedXmlPropertyConfiguration<T, TProperty> Order(int order)
        {
            ChangedOrder = order;
            return this;
        }
    }
}