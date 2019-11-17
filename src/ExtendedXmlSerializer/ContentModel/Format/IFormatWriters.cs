using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	interface IFormatWriters : IParameterizedSource<System.Xml.XmlWriter, IFormatWriter> {}
}