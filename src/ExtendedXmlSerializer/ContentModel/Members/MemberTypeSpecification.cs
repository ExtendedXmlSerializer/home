using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberTypeSpecification : IMemberSpecification
	{
		readonly ISpecification<TypeInfo> _specification;

		public MemberTypeSpecification(ISpecification<TypeInfo> specification) => _specification = specification;

		public bool IsSatisfiedBy(IMember parameter) => _specification.IsSatisfiedBy(parameter.MemberType);
	}
}