using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class AttributeSpecification : DecoratedSpecification<object>, IAttributeSpecification
	{
		public AttributeSpecification(ISpecification<object> specification) : base(specification) {}
	}
}