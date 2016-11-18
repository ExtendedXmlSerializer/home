using System.Xml.Serialization;

namespace ExtendedXmlSerialization.Test.TestObject
{
    public class TestClassWithOrderParameters
    {
        [XmlElement(Order = 2)]
        public string B { get; set; }
        [XmlElement(Order = 1)]
        public string A { get; set; }
    }

    public class TestClassWithOrderParametersExt
    {
        public string D { get; set; }
        [XmlElement(Order = 2)]
        public string B { get; set; }
        [XmlElement(Order = 1)]
        public string A { get; set; }
        public string C { get; set; }
    }
}
