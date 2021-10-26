using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue558Tests
	{
		[Fact]
		public void Verify()
		{
			var subject = new ConfigurationContainer().Create().ForTesting();

			const string document =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue558Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues.DoesNotExist"" />";

			subject.Invoking(x => x.Deserialize<Subject>(document))
			       .Should()
			       .ThrowExactly<InvalidOperationException>()
			       .WithMessage("Could not load assembly 'ExtendedXmlSerializer.Tests.ReportedIssues.DoesNotExist'.  Are you sure it exists?");
		}

		sealed class Subject {}
	}
}