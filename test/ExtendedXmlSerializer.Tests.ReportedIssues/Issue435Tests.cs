using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue435Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new Range<int>(1, 2);
			var serializer = new ConfigurationContainer().Type<Range<int>>().EnableParameterizedContent().Create();
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		public readonly struct Range<T>
		{
			public Range(T min, T max)
			{
				Min = min;
				Max = max;
			}

			public T Min { get; }

			public T Max { get; }
		}
	}
}
