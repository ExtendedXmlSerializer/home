using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using ExtendedXmlSerializer.Tests.TestObject;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
	public class AutoMemberFormatExtensionTests
	{
		[Fact]
		public void VerifyAutoFormatting()
		{
			var support = new SerializationSupport(new ConfigurationContainer().UseAutoFormatting()
			                                                                   .Create());
			var instance = TestClassPrimitiveTypes.Create();
			var data     = support.Serialize(instance);
			Assert.Equal(
			             @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassPrimitiveTypes PropString=""TestString"" PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropDecimalMinValue=""-79228162514264337593543950335"" PropDecimalMaxValue=""79228162514264337593543950335"" PropFloat=""7.4432"" PropFloatNaN=""NaN"" PropFloatPositiveInfinity=""INF"" PropFloatNegativeInfinity=""-INF"" PropFloatMinValue=""-3.40282347E+38"" PropFloatMaxValue=""3.40282347E+38"" PropDouble=""3.4234"" PropDoubleNaN=""NaN"" PropDoublePositiveInfinity=""INF"" PropDoubleNegativeInfinity=""-INF"" PropDoubleMinValue=""-1.7976931348623157E+308"" PropDoubleMaxValue=""1.7976931348623157E+308"" PropEnum=""EnumValue1"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""g"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests"" />",
			             data
			            );
		}

		[Fact]
		public void VerifyAutoFormattingForNullableWithValue()
		{
			var support = new SerializationSupport(new ConfigurationContainer().UseAutoFormatting()
			                                                                   .Create());
			var instance = new TestClassPrimitiveTypesNullable();
			instance.Init();
			var data = support.Serialize(instance);
			var expected =
				@"<?xml version=""1.0"" encoding=""utf-8""?><TestClassPrimitiveTypesNullable PropString=""TestString"" PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropFloat=""7.4432"" PropDouble=""3.4234"" PropEnum=""EnumValue1"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""g"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests"" />";
			data.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void VerifyAutoFormattingForNullableWithNull()
		{
			var support = new SerializationSupport(new ConfigurationContainer().UseAutoFormatting()
			                                                                   .Create());
			var instance = new TestClassPrimitiveTypesNullable();
			instance.InitNull();
			var data = support.Serialize(instance);
			Assert.Equal(
			             @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassPrimitiveTypesNullable xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests"" />",
			             data
			            );
		}

		[Fact]
		public void VerifyAutoFormattingWithLongContent()
		{
			var support = new SerializationSupport(new ConfigurationContainer().UseAutoFormatting()
			                                                                   .Create());
			var instance = TestClassPrimitiveTypes.Create();
			instance.PropString =
				"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed urna sapien, pulvinar et consequat sit amet, fermentum in volutpat. This sentence should break the property out into content.";

			support.Assert(instance,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassPrimitiveTypes PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropDecimalMinValue=""-79228162514264337593543950335"" PropDecimalMaxValue=""79228162514264337593543950335"" PropFloat=""7.4432"" PropFloatNaN=""NaN"" PropFloatPositiveInfinity=""INF"" PropFloatNegativeInfinity=""-INF"" PropFloatMinValue=""-3.40282347E+38"" PropFloatMaxValue=""3.40282347E+38"" PropDouble=""3.4234"" PropDoubleNaN=""NaN"" PropDoublePositiveInfinity=""INF"" PropDoubleNegativeInfinity=""-INF"" PropDoubleMinValue=""-1.7976931348623157E+308"" PropDoubleMaxValue=""1.7976931348623157E+308"" PropEnum=""EnumValue1"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""g"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><PropString>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed urna sapien, pulvinar et consequat sit amet, fermentum in volutpat. This sentence should break the property out into content.</PropString></TestClassPrimitiveTypes>");
		}
	}
}