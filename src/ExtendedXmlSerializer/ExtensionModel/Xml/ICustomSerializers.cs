using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public interface ICustomSerializers : ITypedTable<ISerializer>, ISerializerExtension {}
}