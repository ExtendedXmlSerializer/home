using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Extended Xml Serializer
	/// </summary>
	[UsedImplicitly]
	sealed class ExtendedXmlSerializer : IExtendedXmlSerializer
	{
		readonly ISerializer<System.Xml.XmlReader, System.Xml.XmlWriter> _serializer;

		public ExtendedXmlSerializer(ISerializer<System.Xml.XmlReader, System.Xml.XmlWriter> serializer) =>
			_serializer = serializer;

		public void Serialize(System.Xml.XmlWriter writer, object instance)
			=> _serializer.Serialize(writer, instance);

		public object Deserialize(System.Xml.XmlReader reader) => _serializer.Deserialize(reader);
	}
}