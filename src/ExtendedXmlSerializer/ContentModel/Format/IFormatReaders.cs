using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	/// <summary>
	/// For extension authors.  Specialized selector to retrieve an <see cref="IFormatReader"/> for each deserialization operation.
	/// </summary>
	public interface IFormatReaders : IParameterizedSource<System.Xml.XmlReader, IFormatReader> {}
}