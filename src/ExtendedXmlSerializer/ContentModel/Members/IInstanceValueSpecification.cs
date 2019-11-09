using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface IInstanceValueSpecification : IAllowedValueSpecification
	{
		ISpecification<object> Instance { get; }
	}
}