using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue571Tests
	{
		[Fact]
		public void Verify()
		{
			const string content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Issue571Tests-InputModelExample>
	<Configuration />
	<ModuleStates>
		<Capacity>4</Capacity>
		<Issue571Tests-ModuleState>
			<Name>BUC</Name>
		</Issue571Tests-ModuleState>
	</ModuleStates>
</Issue571Tests-InputModelExample>";
			var subject = new ConfigurationContainer()
			              .EnableImplicitTyping(typeof(InputModelExample), typeof(ModuleState))
			              .WithUnknownContent()
			              .Throw()
			              .Create()
			              .ForTesting();

			subject.Invoking(support => support.Deserialize<InputModelExample>(content)).Should().NotThrow();
		}

		[Fact]
		public void VerifyDefault()
		{
			const string content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Issue571Tests-InputModelExample>
	<Issue571Tests-Configuration xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:member="""">
		<Language>LAD</Language>
		<Section>Section1</Section>
		<Plc>FC-6011-SB1-CPU</Plc>
	</Issue571Tests-Configuration>
	<ModuleStates>
		<Issue571Tests-ModuleState>
			<Name>BUC</Name>
		</Issue571Tests-ModuleState>
	</ModuleStates>
</Issue571Tests-InputModelExample>";
			var subject = new ConfigurationContainer()
			              .EnableImplicitTyping(typeof(InputModelExample))
			              .Create()
			              .ForTesting();

			subject.Deserialize<InputModelExample>(content).ModuleStates.Should().BeEmpty();
		}

		public class InputModelExample
		{
			public Configuration Configuration { get; set; }
			public List<ModuleState> ModuleStates { get; set; } = new List<ModuleState>();
		}

		public class Configuration
		{
			public string Language { get; set; }
			public string Section { get; set; }
			public string Plc { get; set; }
		}

		public class ModuleState
		{
			public string Name { get; set; }
		}
	}
}