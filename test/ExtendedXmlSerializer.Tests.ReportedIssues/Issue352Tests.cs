using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Shared;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue352Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Worker();
			var cycled = new ConfigurationContainer().Create()
			                                         .ForTesting()
			                                         .Cycle(instance);

			cycled.Should().BeEquivalentTo(instance);
		}
	}
}