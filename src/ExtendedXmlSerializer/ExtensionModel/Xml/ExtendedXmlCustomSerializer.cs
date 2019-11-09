using System;
using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	class ExtendedXmlCustomSerializer<T> : IExtendedXmlCustomSerializer<T>
	{
		readonly Func<XElement, T>               _deserialize;
		readonly Action<System.Xml.XmlWriter, T> _serializer;

		public ExtendedXmlCustomSerializer(Func<XElement, T> deserialize,
		                                   Action<System.Xml.XmlWriter, T> serializer)
		{
			_deserialize = deserialize;
			_serializer  = serializer;
		}

		public T Deserialize(XElement xElement) => _deserialize(xElement);

		public void Serializer(System.Xml.XmlWriter xmlWriter, T obj) => _serializer(xmlWriter, obj);
	}
}