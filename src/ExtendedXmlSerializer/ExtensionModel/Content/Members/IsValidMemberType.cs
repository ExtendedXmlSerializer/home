using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	public sealed class IsValidMemberType : ISpecification<IMember>
	{
		readonly IMetadataSpecification _specification;

		public IsValidMemberType(IMetadataSpecification specification) => _specification = specification;

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