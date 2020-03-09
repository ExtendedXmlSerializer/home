using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue372Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().UseOptimizedNamespaces()
			                                             .EnableParameterizedContent()
			                                             .Create()
			                                             .ForTesting();
			var instance = new KeyValuePair<string, object>("123", "123");
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}
	}
}