using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue256Tests
	{
		[Fact]
		void Verify()
		{
			var names = new List<string>();
			var data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue256Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Invalid>Hello World!</Invalid><Message>Hello World!</Message></Issue256Tests-Subject>";
			new ConfigurationContainer().EnableMissingMemberHandling(reader =>
				                                                         names.Add(IdentityFormatter
				                                                                   .Default.Get(reader)))
			                            .Create()
			                            .Deserialize<Subject>(data);
			names.Should()
			     .HaveCount(2)
			     .And.Subject.Should()
			     .Contain("http://www.w3.org/2000/xmlns/:xmlns",
			              "clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests:Invalid");
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}
	}
}