using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Xml.Linq;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class Adapter<T> : IExtendedXmlCustomSerializer
	{
		readonly IExtendedXmlCustomSerializer<T> _configuration;

		public Adapter(IExtendedXmlCustomSerializer<T> configuration) => _configuration = configuration;

		public object Deserialize(XElement xElement) => _configuration.Deserialize(xElement);

		public void Serializer(XmlWriter xmlWriter, object instance)
			=> _configuration.Serializer(xmlWriter, instance.AsValid<T>());
	}
}