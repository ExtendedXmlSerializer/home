using ExtendedXmlSerializer.Core.Sources;
using System.IO;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Responsible for providing a reader per deserialization operation.
	/// </summary>
	public interface IXmlReaderFactory
		: IParameterizedSource<Stream, System.Xml.XmlReader>,
		  IParameterizedSource<TextReader, System.Xml.XmlReader> {}
}