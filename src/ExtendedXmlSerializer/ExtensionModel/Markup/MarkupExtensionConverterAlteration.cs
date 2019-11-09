using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionConverterAlteration : IAlteration<IConverter>
	{
		public static MarkupExtensionConverterAlteration Default { get; } = new MarkupExtensionConverterAlteration();

		MarkupExtensionConverterAlteration() {}

		public IConverter Get(IConverter parameter) => new MarkupExtensionAwareConverter(parameter);
	}
}