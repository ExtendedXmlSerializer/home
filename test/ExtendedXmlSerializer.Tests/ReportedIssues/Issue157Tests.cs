using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue157Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContent()
			                                             .Create()
			                                             .ForTesting();

			serializer.Cycle(new Subject(null))
			          .Another.Should()
			          .BeNull();
		}

		struct Subject
		{
			public Subject(AnotherStruct? another) => Another = another;

			public AnotherStruct? Another { get; }
		}

		struct AnotherStruct
		{
			[UsedImplicitly]
			public AnotherStruct(int number) => Number = number;

			public int Number { [UsedImplicitly] get; }
		}
	}
}