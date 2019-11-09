using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public interface ICustomMemberSerializers : IMemberTable<ISerializer>, ISerializerExtension {}
}