using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Immutable;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue485Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new[] { 1, 2, 3, 4 }.ToImmutableList();

			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .UseOptimizedNamespaces()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}
	}
}