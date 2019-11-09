using System.IO;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public interface IXmlWriterFactory
		: IParameterizedSource<Stream, System.Xml.XmlWriter>,
		  IParameterizedSource<TextWriter, System.Xml.XmlWriter> {}
}