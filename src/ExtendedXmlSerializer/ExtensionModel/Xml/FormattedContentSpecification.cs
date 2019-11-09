using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class FormattedContentSpecification : IFormattedContentSpecification
	{
		public static FormattedContentSpecification Default { get; } = new FormattedContentSpecification();

		FormattedContentSpecification() {}

		public bool IsSatisfiedBy(IFormatReader parameter) => parameter.Get()
		                                                               .AsValid<System.Xml.XmlReader>()
		                                                               .HasAttributes;
	}
}