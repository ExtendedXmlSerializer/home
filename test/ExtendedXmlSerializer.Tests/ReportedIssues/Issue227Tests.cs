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
			           .NotBeNull();
		}
	}
}