using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class AllowedValueSpecification : DecoratedSpecification<object>, IAllowedValueSpecification
	{
		public AllowedValueSpecification(ISpecification<object> specification) : base(specification) {}
	}
}