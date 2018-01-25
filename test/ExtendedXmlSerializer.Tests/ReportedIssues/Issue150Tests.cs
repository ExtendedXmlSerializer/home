using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue150Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().ConfigureType<Subject>()
			                                             .Member(x => x.Message)
			                                             .Register(Serializer.Default)
			                                             .Create()
			                                             .ForTesting();

			var serialize = serializer.Serialize(new Subject {Message = "Hello???"});
			serialize.Should()
			         .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue150Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message></Issue150Tests-Subject>");

			serializer.Deserialize<Subject>(serialize)
			          .Message.Should()
			          .Be("Hello World!!");
		}

		[Fact]
		public void VerifyVerbatim()
		{
			var serializer = new ConfigurationContainer().ConfigureType<Subject>()
			                                             .Member(x => x.Message)
			                                             .Verbatim()
			                                             .Create()
			                                             .ForTesting();

			var subject = new Subject {Message = @"Hello??? (<, &, ', and "")"};
			var serialize = serializer.Serialize(subject);
			serialize.Should()
			         .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue150Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message><![CDATA[Hello??? (<, &, ', and "")]]></Message></Issue150Tests-Subject>");

			serializer.Deserialize<Subject>(serialize)
			          .ShouldBeEquivalentTo(subject);
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}

		sealed class Serializer : ISerializer<string>
		{
			public static Serializer Default { get; } = new Serializer();
			Serializer() {}

			public string Get(IFormatReader parameter) => "Hello World!!";

			public void Write(IFormatWriter writer, string instance)
			{
				writer.Content("Hello World!");
			}
		}
	}
}