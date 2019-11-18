using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Specifications;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ElementSpecification : ISpecification<IInnerContent>
	{
		public static ElementSpecification Default { get; } = new ElementSpecification();

		ElementSpecification() {}

		public bool IsSatisfiedBy(IInnerContent parameter)
			=> parameter.Get()
			            .Get()
			            .AsValid<System.Xml.XmlReader>()
			            .NodeType == XmlNodeType.Element;
	}
}