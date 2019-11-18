using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue186Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().Type<SubjectRequest>()
			                                             .Member(x => x.SomeInterface)
			                                             .Name("YourName")
			                                             .EnableImplicitTypingFromNested<Issue186Tests>()
			                                             .Create()
			                                             .ForTesting();
			var request = new SubjectRequest
			{
				SomeInterface = new Subject2 {Message = "message1", Message2 = "message2"}
			};

			var serialize = serializer.Serialize(request);
			serialize.Should()
			         .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue186Tests-SubjectRequest><YourName xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""Issue186Tests-Subject2""><Message>message1</Message><Message2>message2</Message2></YourName></Issue186Tests-SubjectRequest>");
		}

		public interface ISomeInterface
		{
			string Message { get; set; }
		}

		public class Subject : ISomeInterface
		{
			public string Message { get; set; }
		}

		public class Subject2 : ISomeInterface
		{
			public string Message { get; set; }
			public string Message2 { [UsedImplicitly] get; set; }
		}

		public class SubjectRequest
		{
			public ISomeInterface SomeInterface { get; set; }
		}
	}
}