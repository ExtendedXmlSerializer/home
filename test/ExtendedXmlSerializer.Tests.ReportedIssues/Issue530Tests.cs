using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue530Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new Subject{ List = new List<string>() };

			var container = new ConfigurationContainer().Emit(EmitBehaviors.Classic).Create().ForTesting();

			container.Cycle(instance).Should().BeEquivalentTo(instance);
		}


		sealed class Subject
		{
			public List<string> List { get; set; }
		}
	}
}
