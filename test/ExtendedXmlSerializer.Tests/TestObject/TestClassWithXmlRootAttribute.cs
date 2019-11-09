using System.Xml.Serialization;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	[XmlRoot(ElementName = "TestClass")]
	public class TestClassWithXmlRootAttribute
	{
		public int Id { get; set; }
	}
}