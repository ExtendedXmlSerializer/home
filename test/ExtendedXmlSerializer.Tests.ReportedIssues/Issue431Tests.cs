using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue431Tests
	{
		[Fact]
		public void Verify()
		{
			typeof(HelloWorld).Namespace.Should().BeNull();
			var serializer = new ConfigurationContainer().Create().ForTesting();
			var instance = new HelloWorld { Message = "Hello World!" };
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}
	}
}

sealed class HelloWorld
{
	public string Message { get; set; }
}