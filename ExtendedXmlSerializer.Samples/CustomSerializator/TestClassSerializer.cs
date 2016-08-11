using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Samples.CustomSerializator
{
    public class TestClassSerializer : AbstractCustomSerializator<TestClass>
    {
        public override TestClass Read(XElement element)
        {
            return new TestClass(element.Element("String").Value);
        }

        public override void Write(XmlWriter writer, TestClass obj)
        {
            writer.WriteElementString("String", obj.PropStr);
        }
    }
}
