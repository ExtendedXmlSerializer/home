using System.Xml;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	interface IFormatReaderContexts : IParameterizedSource<XmlReader, IFormatReaderContext> {}
}