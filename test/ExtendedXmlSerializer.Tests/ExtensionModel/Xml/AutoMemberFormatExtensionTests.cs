// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
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
			var support = new SerializationSupport(new ConfigurationContainer().UseAutoFormatting().Create());
			var instance = TestClassPrimitiveTypes.Create();
			var data = support.Serialize(instance);
			Assert.Equal(
				@"<?xml version=""1.0"" encoding=""utf-8""?><TestClassPrimitiveTypes PropString=""TestString"" PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropDecimalMinValue=""-79228162514264337593543950335"" PropDecimalMaxValue=""79228162514264337593543950335"" PropFloat=""7.4432"" PropFloatNaN=""NaN"" PropFloatPositiveInfinity=""INF"" PropFloatNegativeInfinity=""-INF"" PropFloatMinValue=""-3.40282347E+38"" PropFloatMaxValue=""3.40282347E+38"" PropDouble=""3.4234"" PropDoubleNaN=""NaN"" PropDoublePositiveInfinity=""INF"" PropDoubleNegativeInfinity=""-INF"" PropDoubleMinValue=""-1.7976931348623157E+308"" PropDoubleMaxValue=""1.7976931348623157E+308"" PropEnum=""EnumValue1"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""g"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests"" />",
				data
			);
		}

		[Fact]
		public void VerifyAutoFormattingForNullableWithValue()
		{
			var support = new SerializationSupport(new ConfigurationContainer().UseAutoFormatting().Create());
			var instance = new TestClassPrimitiveTypesNullable();
			instance.Init();
			var data = support.Serialize(instance);
			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassPrimitiveTypesNullable PropString=""TestString"" PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropFloat=""7.4432"" PropDouble=""3.4234"" PropEnum=""EnumValue1"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""g"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests"" />";
			data.ShouldBeEquivalentTo(expected);
		}

		[Fact]
		public void VerifyAutoFormattingForNullableWithNull()
		{
			var support = new SerializationSupport(new ConfigurationContainer().UseAutoFormatting().Create());
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
			var support = new SerializationSupport(new ConfigurationContainer().UseAutoFormatting().Create());
			var instance = TestClassPrimitiveTypes.Create();
			instance.PropString =
				"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed urna sapien, pulvinar et consequat sit amet, fermentum in volutpat. This sentence should break the property out into content.";

			support.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassPrimitiveTypes PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropDecimalMinValue=""-79228162514264337593543950335"" PropDecimalMaxValue=""79228162514264337593543950335"" PropFloat=""7.4432"" PropFloatNaN=""NaN"" PropFloatPositiveInfinity=""INF"" PropFloatNegativeInfinity=""-INF"" PropFloatMinValue=""-3.40282347E+38"" PropFloatMaxValue=""3.40282347E+38"" PropDouble=""3.4234"" PropDoubleNaN=""NaN"" PropDoublePositiveInfinity=""INF"" PropDoubleNegativeInfinity=""-INF"" PropDoubleMinValue=""-1.7976931348623157E+308"" PropDoubleMaxValue=""1.7976931348623157E+308"" PropEnum=""EnumValue1"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""g"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><PropString>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed urna sapien, pulvinar et consequat sit amet, fermentum in volutpat. This sentence should break the property out into content.</PropString></TestClassPrimitiveTypes>");


		}
	}
}