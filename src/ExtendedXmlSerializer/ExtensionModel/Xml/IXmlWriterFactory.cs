using ExtendedXmlSerializer.Core.Sources;
using System.IO;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Responsible for providing a writer per serialization operation.
	/// </summary>
	public interface IXmlWriterFactory
		: IParameterizedSource<Stream, System.Xml.XmlWriter>,
		  IParameterizedSource<TextWriter, System.Xml.XmlWriter> {}
}