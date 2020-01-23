using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue358Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer()
			                 .EnableParameterizedContentWithPropertyAssignments()
			                 .EnableReferences()
			                 .Create()
			                 .ForTesting();

			var instance = new vector(new length(11), new length(13));

			var cycled = serializer.Cycle(instance);
			cycled.L1.Value.Should().Be(instance.L1.Value);
			cycled.L2.Value.Should().Be(instance.L2.Value);
		}

		class length
		{
			public length(int value)
			{
				Value = value;
			}

			public int Value { get; }
		}

		private struct vector
		{
			public vector(length l1, length l2)
			{
				L1 = l1;
				L2 = l2;
			}

			public length L1 { get; }
			public length L2 { get; }
		}
	}
}