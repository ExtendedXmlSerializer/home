using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue337Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new TestClass();
			new ConfigurationContainer().Emit(EmitBehaviors.WhenModified)
			                            .Create()
			                            .ForTesting()
			                            .Cycle(instance)
			                            .Should()
			                            .BeEquivalentTo(instance);
		}

		class TestClass
		{
			[UsedImplicitly]
			public string[] Array { get; set; }
		}
	}
}