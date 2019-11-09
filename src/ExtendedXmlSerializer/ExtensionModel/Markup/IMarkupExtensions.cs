using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	interface IMarkupExtensions : IParameterizedSource<IFormatReader, IMarkupExtensionPartsEvaluator> {}
}