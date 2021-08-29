using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue513Tests
	{
		[Fact]
		public void VerifyThreadAwareness()
		{
			var sut      = new ConfigurationContainer().EnableThreadAwareRecursionContent().Create().ForTesting();
			var instance = new Subject { Message = "Hello World!" };
			sut.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		sealed class Subject
		{
			[UsedImplicitly]
			public string Message { get; set; }
		}

	}
}
