using ExtendedXmlSerializer.Core.Parsing;
using FluentAssertions;
using Sprache;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue543Tests
	{
		[Fact]
		public void Verify()
		{
			const string input = "clr-namespace:;assembly=Some-Assembly.Tests";
			var path = ExtendedXmlSerializer.ContentModel.Reflection.AssemblyPathParser.Default.ToParser()
			                                .Parse(input);
			path.Namespace.Should().BeNull();
			path.Path.Should().Be("Some-Assembly.Tests");
		}
	}
}
