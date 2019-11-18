using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue153Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().EnableImplicitTypingFromNested<Issue153Tests>()
			                                             .Create()
			                                             .ForTesting();
			var subject = new Subject {Message = "Hello World!"};
			serializer.Assert(subject,
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue153Tests-Subject><Message>Hello World!</Message></Issue153Tests-Subject>")
			          .Should().BeEquivalentTo(subject);
		}

		sealed class Subject
		{
			public string Message { [UsedImplicitly] get; set; }
		}
	}
}