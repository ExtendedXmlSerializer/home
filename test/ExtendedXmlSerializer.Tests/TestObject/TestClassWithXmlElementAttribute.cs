using System.Xml.Serialization;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassWithXmlElementAttribute
	{
		[XmlElement(ElementName = "Identifier")]
		public int Id { get; set; }
	}
}