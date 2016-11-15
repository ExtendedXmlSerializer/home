using System.Xml.Serialization;

namespace ExtendedXmlSerialization.Test.TestObject
{
    public class TestClassWithXmlElementAttribute
    {
        [XmlElement(ElementName = "Identifier")]
        public int Id { get; set; }
    }
}
