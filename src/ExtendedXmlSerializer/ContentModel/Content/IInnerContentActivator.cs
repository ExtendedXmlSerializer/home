using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	interface IInnerContentActivator : IParameterizedSource<IFormatReader, IInnerContent> {}
}