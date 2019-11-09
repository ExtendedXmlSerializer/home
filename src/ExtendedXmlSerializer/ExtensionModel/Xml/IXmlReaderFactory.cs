using System.IO;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public interface IXmlReaderFactory
		: IParameterizedSource<Stream, System.Xml.XmlReader>,
		  IParameterizedSource<TextReader, System.Xml.XmlReader> {}
}