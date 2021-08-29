using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue511Tests
	{
		[Fact]
		public void Verify()
		{
			var sut      = new ConfigurationContainer().EnableStaticReferenceChecking(false).Create().ForTesting();
			var instance = new Subject { Other = new Other() };
			sut.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		class Subject
		{
			public Other Other { [UsedImplicitly] get; set; }
		}

		class Other
		{
			[UsedImplicitly]
			public Subject Subject { get; set; }
		}
	}
}