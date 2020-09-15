using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue447Tests
	{
		[Fact]
		public void Verify()
		{
			const string content = @"<?xml version=""1.0"" encoding=""utf-8""?><Issue447Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues""><Message><![CDATA[Hello ]]><![CDATA[world!]]></Message><Message2><![CDATA[Not ]]><![CDATA[this.]]></Message2></Issue447Tests-Subject>";

			var serializer = new ConfigurationContainer().Create()
			                                             .ForTesting();

			serializer.Deserialize<Subject>(content)
			          .Message.Should().Be("Hello world!");

			serializer.Deserialize<Subject>(content)
			          .Message2.Should().Be("Not this.");
		}

		sealed class Subject
		{
			[Verbatim]
			public string Message { get;  [UsedImplicitly] set; }

			[Verbatim]
			public string Message2 { get; [UsedImplicitly] set; }
		}
	}
}
