using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue227Tests
	{
		readonly IExtendedXmlSerializer _serializer = new ConfigurationContainer().EnableReferences()
		                                                                          .Create();

		class FooBar
		{
			public string Foo { get; set; } = "Bar";
		}

		[Fact]
		void Verify()
		{
			_serializer.Serialize(new FooBar())
			           .Should()
			           .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue227Tests-FooBar xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Foo>Bar</Foo></Issue227Tests-FooBar>");
		}
	}
}