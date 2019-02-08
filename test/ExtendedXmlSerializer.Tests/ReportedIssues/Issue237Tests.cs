using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue237Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().Emit(EmitBehaviors.Always)
			                                             .UseOptimizedNamespaces()
			                                             .Create()
			                                             .ForTesting();
			var instance = new Subject();
			serializer.Cycle(instance)
			          .ShouldBeEquivalentTo(instance);
		}

		sealed class Subject
		{
			public int[] Array { get; } = null;
		}
	}
}
