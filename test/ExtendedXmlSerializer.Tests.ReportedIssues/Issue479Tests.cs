using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue479Tests
	{
		[Fact]
		public void Verify()
		{
			true.Should().BeTrue();
		}
	}
}
