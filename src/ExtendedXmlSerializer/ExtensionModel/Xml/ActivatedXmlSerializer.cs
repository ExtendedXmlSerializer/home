using System;
using System.Reflection;
using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ActivatedXmlSerializer : Activated<IExtendedXmlCustomSerializer>, IExtendedXmlCustomSerializer
	{
		public ActivatedXmlSerializer(Type objectType, TypeInfo targetType) :
			base(objectType, targetType, typeof(Adapter<>)) {}

		public object Deserialize(XElement xElement) => Throw();

		public void Serializer(System.Xml.XmlWriter xmlWriter, object instance)
		{
			Throw();
		}

		static object Throw() =>
			throw new
				NotSupportedException("This serializer is used as a marker to activate another serializer at runtime.");
	}
}