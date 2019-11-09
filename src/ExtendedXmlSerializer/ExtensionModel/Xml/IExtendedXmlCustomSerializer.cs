using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public interface IExtendedXmlCustomSerializer<T>
	{
		T Deserialize(XElement xElement);

		void Serializer(System.Xml.XmlWriter xmlWriter, T obj);
	}

	public interface IExtendedXmlCustomSerializer
	{
		object Deserialize(XElement xElement);

		void Serializer(System.Xml.XmlWriter xmlWriter, object instance);
	}
}