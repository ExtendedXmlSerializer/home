using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue167Tests
	{
		[Fact]
		public void Verify()
		{
			var container = new ConfigurationContainer().Type<Subject>()
			                                            .Member(x => x.Message)
			                                            .Verbatim()
			                                            .Create()
			                                            .ForTesting();

			var    message = '\0'.ToString();
			Action action  = () => container.WriteLine(new Subject {Message = message});
			action.Should().Throw<ArgumentException>();
		}

		[Fact]
		public void VerifyContent()
		{
			var container = new ConfigurationContainer().Type<Subject>()
			                                            .Member(x => x.Message)
			                                            .Verbatim()
			                                            .WithValidCharacters()
			                                            .Create()
			                                            .ForTesting();

			var message = '\0'.ToString();
			container.Assert(new Subject {Message = message},
			                 @"<?xml version=""1.0"" encoding=""utf-8""?><Issue167Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message><![CDATA[]]></Message></Issue167Tests-Subject>");
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}
	}
}