using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationWithXmlAttributes : BaseTest
    {
        [Fact]
        public void XmlElement()
        {
            var obj = new TestClassWithXmlElementAttribute {Id = 123};

            CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Resources.TestClassWithXmlElementAttribute.xml", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }
        [Fact]
        public void XmlRoot()
        {
            var obj = new TestClassWithXmlRootAttribute { Id = 123 };

            CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Resources.TestClassWithXmlRootAttribute.xml", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }
    }
}
