using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue423Tests
	{
		[Fact]
		public void Verify()
		{
			var subject = new Subject {Values = {"Hello", "World!"}};
			var element = new[] {subject};

			var serializer = new ConfigurationContainer().UseOptimizedNamespaces()
			                                             .EnableImplicitTyping(typeof(Subject))
			                                             .Create()
			                                             .ForTesting();
			serializer.Cycle(element).Should().BeEquivalentTo(element.Cast<Subject>());
		}

		[Fact]
		public void VerifyReported()
		{
			var config = new NewOptionSetFieldInfo
			{
				Values =
				{
					{123, "abc"},
					{456, "def"}
				}
			};
			var element = new[] {config};

			var serializer = new ConfigurationContainer().UseOptimizedNamespaces()
			                                             .EnableImplicitTypingFromPublicNested<Issue423Tests>()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();

			serializer.Cycle(config).Should().BeEquivalentTo(config);
			serializer.Cycle(element).Should().BeEquivalentTo(element.Cast<NewOptionSetFieldInfo>());
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

		sealed class Subject
		{
			[UsedImplicitly]
			public List<string> Values { get; } = new List<string>();
		}
	}
}