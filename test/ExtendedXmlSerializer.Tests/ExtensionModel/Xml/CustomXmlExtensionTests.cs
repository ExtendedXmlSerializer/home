using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using ExtendedXmlSerializer.Tests.TestObject;
using System;
using System.Globalization;
using System.Xml.Linq;
using Xunit;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
	public class CustomXmlExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Type<TestClassWithSerializer>()
			                                             .CustomSerializer(new CustomSerializer())
			                                             .Create();
			var support  = new SerializationSupport(serializer);
			var expected = new TestClassWithSerializer("String", 17);
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithSerializer xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><String>String</String><Int>17</Int></TestClassWithSerializer>"
			                           );
			Assert.Equal(expected.PropInt, actual.PropInt);
			Assert.Equal(expected.PropStr, actual.PropStr);
		}

		class CustomSerializer : IExtendedXmlCustomSerializer<TestClassWithSerializer>
		{
			public TestClassWithSerializer Deserialize(XElement element)
			{
				var xElement  = element.Member("String");
				var xElement1 = element.Member("Int");
				if (xElement != null && xElement1 != null)
				{
					var strValue = xElement.Value;

					var intValue = Convert.ToInt32(xElement1.Value);
					return new TestClassWithSerializer(strValue, intValue);
				}

				throw new InvalidOperationException("Invalid xml for class TestClassWithSerializer");
			}

			public void Serializer(XmlWriter writer, TestClassWithSerializer obj)
			{
				writer.WriteElementString("String", obj.PropStr);
				writer.WriteElementString("Int", obj.PropInt.ToString(CultureInfo.InvariantCulture));
			}
		}
	}
}