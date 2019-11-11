using ExtendedXmlSerializer.Configuration;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue274Tests
	{
		[Fact]
		void Verify()
		{
			new ConfigurationContainer().WithUnknownContent()
			                            .Call(reader => {});
		}
	}
}
