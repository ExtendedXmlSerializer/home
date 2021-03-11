using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Xml;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue507Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().EnableMemberExceptionHandling()
			                                             .WithUnknownContent()
			                                             .Throw()
			                                             .Create()
			                                             .ForTesting();
			var document = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Issue507Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues"">
	<Message>hello world!</Message>
	<DoesNotExist>This is very broken</DoesNotExist>
</Issue507Tests-Subject>";
			serializer.Invoking(x => x.Deserialize<Subject>(document))
			          .Should()
			          .Throw<XmlException>()
			          .Which.LinePosition.Should()
			          .BeGreaterThan(0);
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}
	}
}