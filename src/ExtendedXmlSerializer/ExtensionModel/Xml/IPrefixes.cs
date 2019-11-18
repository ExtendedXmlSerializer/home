using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// A prefix store, keyed by <see cref="System.Xml.XmlWriter"/>.
	/// </summary>
	public interface IPrefixes : IParameterizedSource<System.Xml.XmlWriter, IPrefix> {}
}