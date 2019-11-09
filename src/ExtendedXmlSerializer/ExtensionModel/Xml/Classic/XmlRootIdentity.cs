using System.Xml.Serialization;
using ExtendedXmlSerializer.ContentModel.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class XmlRootIdentity : TypeIdentity<XmlRootAttribute>
	{
		public static XmlRootIdentity Default { get; } = new XmlRootIdentity();

		XmlRootIdentity() : base(attribute => new Key(attribute.ElementName, attribute.Namespace)) {}
	}
}