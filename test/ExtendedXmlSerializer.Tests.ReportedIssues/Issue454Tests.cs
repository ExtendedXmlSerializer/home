using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue454Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .UseOptimizedNamespaces()
			                                             .EnableReferences()
			                                             .Create()
			                                             .ForTesting();

			var myRef = new Foo();
			var instance = new Foo
			{
				Ref1 = myRef,
				Ref2 = myRef
			};

			var first  = serializer.Serialize(serializer.Cycle(instance));
			var second = serializer.Serialize(serializer.Cycle(instance));

			first.Should().Be(second);
		}

		public class Foo
		{
			public Foo Ref1 { get; set; }
			public Foo Ref2 { get; set; }
		}
	}
}