using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	/// <summary>
	/// Used to determine if a member is valid.
	/// </summary>
	public sealed class IsValidMemberType : ISpecification<IMember>
	{
		readonly IMetadataSpecification _specification;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExtendedXmlSerializer.ExtensionModel.Content.Members.IsValidMemberType"/> class.
		/// </summary>
		/// <param name="specification">The specification.</param>
		public IsValidMemberType(IMetadataSpecification specification) => _specification = specification;

		/// <inheritdoc />
		public bool IsSatisfiedBy(IMember parameter)
		{
			switch (parameter.Metadata)
			{
				case FieldInfo field:
					return _specification.IsSatisfiedBy(field);
				case PropertyInfo property:
					return _specification.IsSatisfiedBy(property);
				default:
					return false;
			}
		}
	}
}