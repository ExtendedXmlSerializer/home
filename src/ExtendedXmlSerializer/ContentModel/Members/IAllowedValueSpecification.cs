using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// A specific specification implementation that determines if a value should be emitted.
	/// </summary>
	public interface IAllowedValueSpecification : ISpecification<object> {}
}