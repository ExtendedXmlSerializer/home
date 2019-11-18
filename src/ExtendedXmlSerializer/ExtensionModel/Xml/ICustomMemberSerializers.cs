using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Specialized selector that manages custom serializer registration for a specific member.
	/// </summary>
	public interface ICustomMemberSerializers : IMemberTable<ContentModel.ISerializer>, ISerializerExtension {}
}