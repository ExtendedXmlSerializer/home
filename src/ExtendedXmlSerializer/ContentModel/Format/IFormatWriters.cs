using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	/// <summary>
	/// For extension authors.  Specialized selector to retrieve an <see cref="IFormatWriter"/> for each serialization operation.
	/// </summary>
	public interface IFormatWriters : IParameterizedSource<System.Xml.XmlWriter, IFormatWriter> {}
}