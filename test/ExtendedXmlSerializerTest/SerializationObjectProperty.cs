using System;
using System.Collections.Generic;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationObjectProperty :BaseTest
    {
        [Fact]
        public void TestClassWithObjectProperty()
        {
            var obj = new List<TestClassWithObjectProperty>
            {
                new TestClassWithObjectProperty {TestProperty = 1234},
                new TestClassWithObjectProperty {TestProperty = "Abc"},
                new TestClassWithObjectProperty {TestProperty = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 2)}
            };
            //CheckCompatibilityWithDefaultSerializator(obj);
            CheckSerializationAndDeserializationByXml(
 @"<ArrayOfTestClassWithObjectProperty>
  <TestClassWithObjectProperty type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithObjectProperty"">
    <TestProperty type=""System.Int32"">1234</TestProperty>
  </TestClassWithObjectProperty>
  <TestClassWithObjectProperty type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithObjectProperty"">
    <TestProperty type=""System.String"">Abc</TestProperty>
  </TestClassWithObjectProperty>
  <TestClassWithObjectProperty type=""ExtendedXmlSerialization.Test.TestObject.TestClassWithObjectProperty"">
    <TestProperty type=""System.Guid"">00000001-0002-0003-0405-060708090002</TestProperty>
  </TestClassWithObjectProperty>
</ArrayOfTestClassWithObjectProperty>", obj);
        }
    }
}
