using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue427Tests
	{
		[Fact]
		public void Verify()
		{
			var config = new NewFieldInfo(FieldType.OptionSet)
			{
				Name       = "ASDF",
				TargetName = "What?"
			};
			var element = new[] {config};

			var serializer = new ConfigurationContainer().UseOptimizedNamespaces()
			                                             .EnableImplicitTypingFromPublicNested<Issue427Tests>()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();

			serializer.Cycle(config).Should().BeEquivalentTo(config);
			serializer.Cycle(element).Should().BeEquivalentTo(element.Cast<NewFieldInfo>());

		}

		public class ExistingOptionSetFieldInfo : NewFieldInfo
		{
			public ExistingOptionSetFieldInfo() : base(FieldType.OptionSet)
			{
				IsExistsOnSource = true;
			}

			public bool IsExistsOnSource { get; protected set; }

			public string OptionSetName { get; set; }
		}

		public enum FieldType
		{
			OptionSet
		}

		public class NewFieldInfo
		{
			public NewFieldInfo(FieldType type)
			{
				Type = type;
			}

			public FieldType Type { get; }

			public string Name { get; set; }

			public string Description { get; set; } = null;

			public bool IsRequired { get; set; }

			public string TargetName { get; set; }

			public bool ShouldCreateFieldOnTarget { get; set; }
		}

		public class NewOptionSetFieldInfo : ExistingOptionSetFieldInfo
		{
			public NewOptionSetFieldInfo()
			{
				IsExistsOnSource = false;
			}

			public Dictionary<int, string> Values { get; } = new Dictionary<int, string>();
		}

	}
}
