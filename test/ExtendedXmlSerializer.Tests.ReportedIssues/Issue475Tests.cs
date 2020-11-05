using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue475Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Type<Subject>().Member(x => x.NoMessage).Ignore().Create();

			var instance = new Subject { Message = "Hello World!", NoMessage = "Ignored" };

			var cycled = serializer.Cycle(instance);
			cycled.NoMessage.Should().BeNullOrEmpty();
			cycled.Message.Should().Be(instance.Message);

		}

		sealed class Subject
		{
			public string Message { get; set; }

			public string NoMessage { get; set; }
		}
	}
}