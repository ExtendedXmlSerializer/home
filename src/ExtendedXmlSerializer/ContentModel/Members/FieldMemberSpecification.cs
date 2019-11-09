using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class FieldMemberSpecification : ISpecification<FieldInfo>
	{
		readonly ISpecification<MemberInfo> _defined;

		public FieldMemberSpecification(ISpecification<MemberInfo> defined)
		{
			_defined = defined;
		}

		public bool IsSatisfiedBy(FieldInfo parameter)
			=>
				!parameter.IsStatic && (!parameter.IsInitOnly || !parameter.IsLiteral) &&
				(parameter.IsPublic || _defined.IsSatisfiedBy(parameter));
	}
}