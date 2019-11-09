using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public interface ICustomXmlSerializers : ITypedTable<IExtendedXmlCustomSerializer>, ISerializerExtension {}
}