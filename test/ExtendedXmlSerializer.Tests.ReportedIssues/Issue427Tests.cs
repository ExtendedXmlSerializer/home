using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue427Tests
	{
		[Fact]
		public void Verify()
		{
			var config = new NewTypeInfo(FieldType.Int)
			{
				Name       = "abc"
			};

			var element = new[] {config};

			var serializer = new ConfigurationContainer().UseOptimizedNamespaces()
			                                             .EnableImplicitTypingFromPublicNested<Issue427Tests>()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();

			serializer.Cycle(config).Should().BeEquivalentTo(config);
			serializer.Cycle(element).Should().BeEquivalentTo(element.Cast<NewTypeInfo>());

		}

		public enum FieldType
		{
			Int,
			Bool
		}

		public class NewTypeInfo
		{
			public NewTypeInfo(FieldType type)
			{
				Type = type;
			}

			public FieldType Type { get; }

			public string Name { get; set; }
		}

	}
}
