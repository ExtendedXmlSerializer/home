using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue358Tests_Extended
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContentWithPropertyAssignments()
			                                             .EnableReferences()
			                                             .Create()
			                                             .ForTesting();

			var length   = new length(11);
			var instance = new vector(length, length);

			var cycled = serializer.Cycle(instance);
			cycled.L1.Should().BeSameAs(cycled.L2);
		}

		class length
		{
			public length(int value)
			{
				Value = value;
			}

			public int Value { get; }
		}

		class vector
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
