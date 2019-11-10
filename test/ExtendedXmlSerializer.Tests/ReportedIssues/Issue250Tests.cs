using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue250Tests
	{
		[Fact]
		void Verify()
		{
			var support = new ConfigurationContainer().EnableParameterizedContent()
			                                          .Create()
			                                          .ForTesting();

			var instance = new DataHolder {Lenght = 13};

			support.Cycle(instance)
			       .Should()
			       .BeEquivalentTo(instance);
		}

		public class DataHolder
		{
			public int Lenght { [UsedImplicitly] get; set; } = 5;
		}
	}
}