using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// A specification that determines if a provided value should emit as an attribute or content.
	/// </summary>
	public interface IAttributeSpecification : ISpecification<object> {}
}