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

            CheckSerializationAndDeserializationByXml(
@"<TestClassWithXmlElementAttribute type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithXmlElementAttribute"">
  <Identifier>123</Identifier>
</TestClassWithXmlElementAttribute>", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }

        [Fact]
        public void XmlElementWithOrder()
        {
            var obj = new TestClassWithOrderParameters {A = "A", B = "B"};
            CheckCompatibilityWithDefaultSerializator(obj);
            CheckSerializationAndDeserializationByXml(
@"<TestClassWithOrderParameters type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithOrderParameters"">
  <A>A</A>
  <B>B</B>
</TestClassWithOrderParameters>", obj);
        }

        [Fact]
        public void XmlElementWithOrderExt()
        {
            var obj = new TestClassWithOrderParametersExt { A = "A", B = "B", C = "C", D = "D" };

            CheckSerializationAndDeserializationByXml(
@"<TestClassWithOrderParametersExt type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithOrderParametersExt"">
  <A>A</A>
  <B>B</B>
  <D>D</D>
  <C>C</C>
</TestClassWithOrderParametersExt>", obj);
        }

        [Fact]
        public void TestClassInheritanceWithOrder()
        {
            var obj = new TestClassInheritanceWithOrder();
            obj.Init();
            CheckCompatibilityWithDefaultSerializator(obj);
            CheckSerializationAndDeserializationByXml(
 @"<TestClassInheritanceWithOrder type=""ExtendedXmlSerialization.Test.TestObject.TestClassInheritanceWithOrder"">
    <Id2>3</Id2>  
    <Id>2</Id>
</TestClassInheritanceWithOrder>", obj);
        }

        [Fact]
        public void TestClassInterfaceInheritanceWithOrder()
        {
            var obj = new TestClassInterfaceInheritanceWithOrder {Id = 1, Id2 = 2};
            CheckCompatibilityWithDefaultSerializator(obj);
            CheckSerializationAndDeserializationByXml(
 @"<TestClassInterfaceInheritanceWithOrder type=""ExtendedXmlSerialization.Test.TestObject.TestClassInterfaceInheritanceWithOrder"">
    <Id2>2</Id2>  
    <Id>1</Id>
</TestClassInterfaceInheritanceWithOrder>", obj);
        }

        [Fact]
        public void XmlRoot()
        {
            var obj = new TestClassWithXmlRootAttribute { Id = 123 };

            CheckSerializationAndDeserializationByXml(
@"<TestClass type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithXmlRootAttribute"">
  <Id>123</Id>
</TestClass>", obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }
    }
}
