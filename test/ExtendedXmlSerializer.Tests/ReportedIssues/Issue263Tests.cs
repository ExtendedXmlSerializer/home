using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue263Tests
	{
		[Fact]
		public void ShouldDeserializeXMLtoObject()
		{
			const string xmlstring =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue263Tests-FooBar><Foo>Test 1</Foo><Bar>Test 2</Bar></Issue263Tests-FooBar>";
			var foobar = new FooBar {Foo = "Test 1", Bar = "Test 2"};
			var serializer = new ConfigurationContainer().EnableImplicitTypingFromNested<Issue263Tests>()
			                                             .Create();
			var result = serializer.Deserialize<FooBar>(xmlstring);
			result.Foo.Should()
			      .Be(foobar.Foo, "Deserialization failed with prop Foo");

			result.Bar.Should()
			      .Be(foobar.Bar, "Deserialization failed with prop Bar");
		}

		public class FooBar
		{
			public string Foo { get; set; }
			public string Bar { get; set; }
		}
	}
}