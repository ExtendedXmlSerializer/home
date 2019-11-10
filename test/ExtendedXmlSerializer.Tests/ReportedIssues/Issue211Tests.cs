using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
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
			                            .Cycle(instance)
			                            .Message
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

		[Fact]
		void VerifyConfigurationWithReferences()
		{
			var instance = new Subject {Message = null};
			new ConfigurationContainer().Emit(EmitBehaviors.Assigned)
			                            .EnableReferences()
			                            .Create()
			                            .Cycle(instance)
			                            .Message
			                            .Should()
			                            .BeNull();
		}

		[Fact]
		void VerifySubjectWithDate()
		{
			var instance = new SubjectWithDate {Date = null};
			new ConfigurationContainer().Emit(EmitBehaviors.Assigned)
			                            .Create()
			                            .Cycle(instance)
			                            .Date
			                            .Should()
			                            .Be(null);
		}

		sealed class Subject
		{
			public const string DefaultValue = "Default Value";
			public string Message { get; set; } = DefaultValue;
		}

		public class SubjectWithDate
		{
			public DateTime? Date { get; set; } = DateTime.MinValue;
		}
	}
}