using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// The root Xml serializer component.  This is what is created by the configuration container and is used to serialize
	/// objects from memory into a destination and deserialize objects into memory from a source.
	/// </summary>
	public interface IExtendedXmlSerializer : ISerializer {}
}