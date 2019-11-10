using System.Xml.Serialization;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue155Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Create()
			                                             .ForTesting();

			var instance = new Subject
				{Items = new object[] {"Hello", "World", 6776, new URL(), new TranslatableWebFeatures()}};
			serializer.Cycle(instance)
			          .Should().BeEquivalentTo(instance);
		}

		public sealed class Subject
		{
			[XmlElement("Tenure", typeof(string), DataType  = "integer", IsNullable = true),
			 XmlElement("Video360", typeof(URL), IsNullable = true),
			 XmlElement("VideoUrl", typeof(URL), IsNullable = true),
			 XmlElement("WebFeatures", typeof(TranslatableWebFeatures)), XmlChoiceIdentifier("ItemsElementName")]
			public object[] Items { get; set; }
		}

		// ReSharper disable once InconsistentNaming
		sealed class URL {}

		sealed class TranslatableWebFeatures {}
	}
}