using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue175Tests
	{
		[Fact]
		public void Verify()
		{
			var element = new MyElementType
			{
				UniqueId = "Message"
			};
			var myMessage = new MyMessage
			{
				MyElement = element
			};
			var support = new ConfigurationContainer().InspectingType<MyMessage>()
			                                          .ForTesting();
			support.Assert(myMessage,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><myMessage xmlns=""http://namespace/file.xsd""><myElement uniqueId=""Message"" /></myMessage>");
			support.Cycle(myMessage)
			       .Should()
			       .BeEquivalentTo(myMessage);

			support.Cycle(element)
			       .Should()
			       .BeEquivalentTo(element);
		}

		[Fact]
		public void None()
		{
			var support = new ConfigurationContainer().InspectingType<None>()
			                                          .ForTesting();
			support.Assert(new None {UniqueId = "123"},
			               @"<?xml version=""1.0"" encoding=""utf-8""?><None uniqueId=""123"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" />");
		}

		[Fact]
		public void VerifyRead()
		{
			const string xml = @"<?xml version=""1.0""?>
						<myMessage xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://namespace/file.xsd"">
							<myElement uniqueId=""12345"" />
						</myMessage>";

			new ConfigurationContainer().InspectingType<MyMessage>()
			                            .ForTesting()
			                            .Deserialize<MyMessage>(xml)
			                            .MyElement.UniqueId.Should()
			                            .Be("12345");
		}

		[Fact]
		void VerifyCrossAttributeCombination()
		{
			const string xml = @"<?xml version=""1.0""?>
						<FooBar xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://namespace/file.xsd"">
							<num>12345</num>
						</FooBar>";

			new ConfigurationContainer().InspectingType<Foo>()
			                            .ForTesting()
			                            .Deserialize<Foo>(xml).Number.Should()
			                            .Be(12345);
		}
	}

	[Serializable]
	[XmlType(AnonymousType = true, Namespace      = "http://namespace/file.xsd")]
	[XmlRoot(ElementName   = "FooBar", IsNullable = false)]
	public class Foo : IFoo
	{
		[XmlElement("num")]
		public int Number { get; set; }
	}

	public interface IFoo {}

	[Serializable, XmlType(AnonymousType = true, Namespace                         = "http://namespace/file.xsd"),
	 XmlRoot("myMessage", Namespace      = "http://namespace/file.xsd", IsNullable = false)]
	public class MyMessage
	{
		/// <remarks />
		[XmlElement("myElement")]
		public MyElementType MyElement { get; set; }
	}

	/// <remarks />
	[XmlType(Namespace = "http://namespace/file.xsd")]
	public class MyElementType
	{
		/// <remarks />
		[XmlAttribute("uniqueId")]
		public string UniqueId { get; set; }
	}

	[XmlType(Namespace = "")]
	public class None
	{
		/// <remarks />
		[XmlAttribute("uniqueId")]
		public string UniqueId { get; set; }
	}
}