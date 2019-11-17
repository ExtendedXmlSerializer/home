using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// Baseline decorated metadata specification for convenience.
	/// </summary>
	public class MetadataSpecification : IMetadataSpecification
	{
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo>    _field;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="property">The specification to use for properties.</param>
		/// <param name="field">The specification to use for fields.</param>
		public MetadataSpecification(ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_property = property;
			_field    = field;
		}

		/// <inheritdoc />
		public bool IsSatisfiedBy(PropertyInfo parameter) => _property.IsSatisfiedBy(parameter);

		/// <inheritdoc />
		public bool IsSatisfiedBy(FieldInfo parameter) => _field.IsSatisfiedBy(parameter);
	}
}