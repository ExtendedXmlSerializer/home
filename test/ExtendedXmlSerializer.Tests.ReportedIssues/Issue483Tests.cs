using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue483Tests
	{
		[Fact]
		public void Verify()
		{
			new ConfigurationContainer().Type<Issue483Tests>().Root.With<MigrationsExtension>().Should().NotBeNull();

		}


	}
}
