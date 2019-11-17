using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	interface IFormatReaders : IParameterizedSource<System.Xml.XmlReader, IFormatReader> {}
}