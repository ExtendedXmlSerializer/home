using System.Xml;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;

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