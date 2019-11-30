using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using System.Xml;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue282Tests_README
	{
		[Fact]
		void Verify()
		{
			IExtendedXmlSerializer serializer = new ConfigurationContainer().UseAutoFormatting()
			                                                                .EnableImplicitTyping(typeof(Subject))
			                                                                .Create();

			var instance = new Subject {Message = "Hello World!", Number = 42};
			var document = serializer.Serialize(new XmlWriterSettings {Indent = true},
			                                    instance);
			document.Should()
			        .Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Issue282Tests_README-Subject Number=""42"" Message=""Hello World!"" />");
			serializer.Deserialize<Subject>(document)
			          .Should()
			          .BeEquivalentTo(instance);
		}

		class Subject
		{
			public int Number { get; set; }
			public string Message { get; set; }
		}
	}
}