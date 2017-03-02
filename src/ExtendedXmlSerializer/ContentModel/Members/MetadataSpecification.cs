using System.Reflection;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class MetadataSpecification : IMetadataSpecification
	{
		readonly static MemberSpecification<PropertyInfo> Property =
			new MemberSpecification<PropertyInfo>(PropertyMemberSpecification.Default);

		readonly static MemberSpecification<FieldInfo> Field =
			new MemberSpecification<FieldInfo>(FieldMemberSpecification.Default);

		public static MetadataSpecification Default { get; } = new MetadataSpecification();
		MetadataSpecification() : this(Property, Field) {}

		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		public MetadataSpecification(ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_property = property;
			_field = field;
		}

		public bool IsSatisfiedBy(PropertyInfo parameter) => _property.IsSatisfiedBy(parameter);

		public bool IsSatisfiedBy(FieldInfo parameter) => _field.IsSatisfiedBy(parameter);
	}
}