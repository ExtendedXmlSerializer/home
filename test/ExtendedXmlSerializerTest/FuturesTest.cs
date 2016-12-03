using System.Collections.Generic;
using ExtendedXmlSerialization.Test.TestObject;
using ExtendedXmlSerialization.Write;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class FuturesTest
    {
        [Fact]
        public void VerifyAutoAttributes()
        {
            var host = new SerializationToolsFactoryHost();
            var services = new List<object>();
            var writings = new WritingFactory(host, services);
            var plan = AutoAttributeWritePlanComposer.Default.Compose();
            var serializer = new ExtendedXmlSerializer(host, services, new AssignmentFactory(host), writings, new Serializer(plan, writings));
            var instance = new TestClassPrimitiveTypes().Init();
            var data = serializer.Serialize(instance);
            Assert.Equal(
                @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassPrimitiveTypes type=""ExtendedXmlSerialization.Test.TestObject.TestClassPrimitiveTypes"" PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropDecimalMinValue=""-79228162514264337593543950335"" PropDecimalMaxValue=""79228162514264337593543950335"" PropFloat=""7.4432"" PropFloatNaN=""NaN"" PropFloatPositiveInfinity=""INF"" PropFloatNegativeInfinity=""-INF"" PropFloatMinValue=""-3.40282347E+38"" PropFloatMaxValue=""3.40282347E+38"" PropDouble=""3.4234"" PropDoubleNaN=""NaN"" PropDoublePositiveInfinity=""INF"" PropDoubleNegativeInfinity=""-INF"" PropDoubleMinValue=""-1.7976931348623157E+308"" PropDoubleMaxValue=""1.7976931348623157E+308"" PropEnum=""EnumValue1"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""103"" PropString=""TestString"" />", data
                );
        }

        [Fact]
        public void VerifyAutoAttributesWithLongContent()
        {
            var host = new SerializationToolsFactoryHost();
            var services = new List<object>();
            var writings = new WritingFactory(host, services);
            var plan = AutoAttributeWritePlanComposer.Default.Compose();
            var serializer = new ExtendedXmlSerializer(host, services, new AssignmentFactory(host), writings, new Serializer(plan, writings));
            var instance = new TestClassPrimitiveTypes().Init();
            instance.PropString = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed urna sapien, pulvinar et consequat sit amet, fermentum in volutpat. This sentence should break the property out into content.";
            var data = serializer.Serialize(instance);
            Assert.Equal(
                @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassPrimitiveTypes type=""ExtendedXmlSerialization.Test.TestObject.TestClassPrimitiveTypes"" PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropDecimalMinValue=""-79228162514264337593543950335"" PropDecimalMaxValue=""79228162514264337593543950335"" PropFloat=""7.4432"" PropFloatNaN=""NaN"" PropFloatPositiveInfinity=""INF"" PropFloatNegativeInfinity=""-INF"" PropFloatMinValue=""-3.40282347E+38"" PropFloatMaxValue=""3.40282347E+38"" PropDouble=""3.4234"" PropDoubleNaN=""NaN"" PropDoublePositiveInfinity=""INF"" PropDoubleNegativeInfinity=""-INF"" PropDoubleMinValue=""-1.7976931348623157E+308"" PropDoubleMaxValue=""1.7976931348623157E+308"" PropEnum=""EnumValue1"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""103""><PropString>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed urna sapien, pulvinar et consequat sit amet, fermentum in volutpat. This sentence should break the property out into content.</PropString></TestClassPrimitiveTypes>", data
                );
        }

        [Fact]
        public void CustomWritePlanForListsWithInheritance()
        {
            
        }

        [Fact]
        public void WriterExtensionForAttachedProperties()
        {
            
        }
    }
}
