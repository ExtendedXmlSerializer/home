using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class ConditionalInnerContentHandler : IInnerContentHandler
	{
		readonly ISpecification<IInnerContent> _specification;
		readonly IInnerContentHandler          _instance;

		public ConditionalInnerContentHandler(ISpecification<IInnerContent> specification,
		                                      IInnerContentHandler instance)
		{
			_specification = specification;
			_instance      = instance;
		}

		public bool IsSatisfiedBy(IInnerContent parameter)
			=> _specification.IsSatisfiedBy(parameter) && _instance.IsSatisfiedBy(parameter);
	}
}