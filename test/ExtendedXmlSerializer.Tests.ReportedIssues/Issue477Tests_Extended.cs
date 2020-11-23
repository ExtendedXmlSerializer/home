using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue477Tests_Extended
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Type<DateTime>()
			                                             .Register()
			                                             .Serializer()
			                                             .ByCalling((writer, date) => writer.Content(date.ToString("yyyy-MM-dd")),
			                                                        null)
			                                             .Type<DateTime?>()
			                                             .Register()
			                                             .Serializer()
			                                             .ByCalling((writer, date) => writer.Content(date?.ToString("yyyy-MM-dd") ?? string.Empty), null)
			                                             .Create()
			                                             .ForTesting();

			var instance = new Test();
			var content = serializer.Serialize(instance);

			content.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue477Tests_Extended-Test xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues""><Date1>2020-11-23</Date1><Date3>2020-11-23</Date3></Issue477Tests_Extended-Test>");


		}

		class Test
		{
			public DateTime Date1 { get; set; } = DateTime.Now;  // Formatted OK
			public DateTime? Date2 { get; set; } = null;         // Is not emitted = OK
			public DateTime? Date3 { get; set; } = DateTime.Now; // Completety ignores configured formatting
		}
	}
}