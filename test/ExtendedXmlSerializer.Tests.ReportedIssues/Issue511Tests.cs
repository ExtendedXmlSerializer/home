using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue511Tests
	{
		[Fact]
		public void Verify()
		{
			var sut      = new ConfigurationContainer().WithoutReferenceChecking().Create().ForTesting();
			var instance = new Subject { Other = new Other() };
			sut.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		class Subject
		{
			public Other Other { get; set; }
		}

		class Other
		{
			public Subject Subject { get; set; }
		}
	}
}