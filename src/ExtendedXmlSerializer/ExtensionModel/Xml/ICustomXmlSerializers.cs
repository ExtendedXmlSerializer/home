using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Specialized selector for v1 functionality.  This is unsupported and should use the v2
	/// <see cref="ICustomSerializers"/> equivalent instead.
	/// </summary>
	public interface ICustomXmlSerializers : ITypedTable<IExtendedXmlCustomSerializer>, ISerializerExtension {}
}