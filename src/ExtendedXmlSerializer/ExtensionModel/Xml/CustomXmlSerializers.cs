using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class CustomXmlSerializers : Metadata<TypeInfo, IExtendedXmlCustomSerializer>, ICustomXmlSerializers
	{
		public CustomXmlSerializers() : base(ReflectionModel.Defaults.TypeComparer) {}
	}
}