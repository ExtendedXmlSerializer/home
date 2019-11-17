namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public interface ISerializer
	{
		object Deserialize(System.Xml.XmlReader reader);

		void Serialize(System.Xml.XmlWriter writer, object instance);
	}
}