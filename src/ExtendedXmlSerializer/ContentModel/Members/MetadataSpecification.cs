using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	public class MetadataSpecification : IMetadataSpecification
	{
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo>    _field;

		public MetadataSpecification(ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_property = property;
			_field    = field;
		}

		public bool IsSatisfiedBy(PropertyInfo parameter) => _property.IsSatisfiedBy(parameter);

		public bool IsSatisfiedBy(FieldInfo parameter) => _field.IsSatisfiedBy(parameter);
	}
}