using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Specialized selector that manages custom serializer registration for a specific type.
	/// </summary>
	public interface ICustomSerializers : ITypedTable<ContentModel.ISerializer>, ISerializerExtension {}
}