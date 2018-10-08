using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue211Tests
	{
		[Fact]
		void VerifyDefaultBehavior()
		{
			var instance = new Subject {Message = null};
			new ConfigurationContainer().Create()
			                            .Cycle(instance).Message
			                            .Should()
			                            .Be(Subject.DefaultValue);
		}

		[Fact]
		void VerifyConfiguration()
		{
			var instance = new Subject {Message = null};
			new ConfigurationContainer().Emit(EmitBehaviors.Assigned)
			                            .Create()
			                            .Cycle(instance)
			                            .Message
			                            .Should()
			                            .BeNull();
		}

		sealed class Subject
		{
			public const string DefaultValue = "Default Value";
			public string Message { get; set; } = DefaultValue;
		}
	}
}