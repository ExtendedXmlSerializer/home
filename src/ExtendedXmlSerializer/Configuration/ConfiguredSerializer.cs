using ExtendedXmlSerializer.ExtensionModel.Xml;
using XmlReader = System.Xml.XmlReader;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.Configuration
{
	public class ConfiguredSerializer<T> : DecoratedSerializer<XmlReader, XmlWriter>, IExtendedXmlSerializer
		where T : class, IConfigurationProfile
	{
		public ConfiguredSerializer() : this(ConfiguredContainer.New<T>()
		                                                        .Create()) {}

		public ConfiguredSerializer(ISerializer<XmlReader, XmlWriter> serializer) : base(serializer) {}
	}
}