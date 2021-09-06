using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue550Tests
	{
		[Fact]
		public void Verify()
		{
			var configuration = new ConfigurationContainer();
			configuration.UseAutoFormatting();
			var instance = new Subject
			{
				Struct = new ItemStruct()
			};

			var serializer   = configuration.Create();
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);

		}

		class Subject
		{

			public ItemStruct? Struct;

		}
		struct ItemStruct
		{}

	}
}
