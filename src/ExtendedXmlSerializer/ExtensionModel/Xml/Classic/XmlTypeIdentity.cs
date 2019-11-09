using System.Xml.Serialization;
using ExtendedXmlSerializer.ContentModel.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class XmlTypeIdentity : TypeIdentity<XmlTypeAttribute>
	{
		public static XmlTypeIdentity Default { get; } = new XmlTypeIdentity();

		XmlTypeIdentity() : base(attribute => new Key(attribute.TypeName, attribute.Namespace)) {}
	}
}