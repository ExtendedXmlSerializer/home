using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue515Tests
	{
		[Fact]
		public void Verify()
		{
			var sut = new ConfigurationContainer().Type<string>().Ignore().Create().ForTesting();
			var instance = new Subject() { Message = "hello world!", Number = 123};

			var subject = sut.Cycle(instance);
			subject.Message.Should().BeNull();
			subject.Number.Should().Be(123);
		}

		sealed class Subject
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}
	}
}
