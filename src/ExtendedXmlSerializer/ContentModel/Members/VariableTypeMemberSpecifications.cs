using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using VariableTypeSpecification = ExtendedXmlSerializer.ReflectionModel.VariableTypeSpecification;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class VariableTypeMemberSpecifications : ReferenceCacheBase<IMember, IVariableTypeSpecification>,
	                                                IVariableTypeMemberSpecifications
	{
		public static VariableTypeMemberSpecifications Default { get; } = new VariableTypeMemberSpecifications();

		VariableTypeMemberSpecifications() : this(new MemberTypeSpecification(VariableTypeSpecification.Default)) {}

		readonly ISpecification<IMember> _specification;

		public VariableTypeMemberSpecifications(ISpecification<IMember> specification)
			=> _specification = specification;

		protected override IVariableTypeSpecification Create(IMember parameter)
			=> _specification.IsSatisfiedBy(parameter)
				   ? Reflection.VariableTypeSpecification.Defaults.Get(parameter.MemberType)
				   : null;
	}
}