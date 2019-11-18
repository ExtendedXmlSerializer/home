using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Drawing;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue249Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new DataHolder();

			new ConfigurationContainer().EnableAllConstructors()
			                            .EnableParameterizedContent()
			                            .EnableReferences()
			                            .Create()
			                            .ForTesting()
			                            .Cycle(instance)
			                            .Should().BeEquivalentTo(instance);
		}

		sealed class DataHolder
		{
			[UsedImplicitly]
			public Size Size { get; set; } = new Size(100, 200);
		}
	}
}