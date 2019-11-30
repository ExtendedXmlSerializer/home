using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue282Tests_Converters
	{
		[Fact]
		void Verify()
		{
			IExtendedXmlSerializer serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Subject))
			                                                                .Type<Subject>()
			                                                                .Register()
			                                                                .Converter()
			                                                                .Using(SubjectConverter.Default)
			                                                                .Create();

			var instance = new Subject {Message = "Hello World!"};
			string document = serializer.Serialize(instance);
			document.Should()
			        .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue282Tests_Converters-Subject>Hello World!</Issue282Tests_Converters-Subject>");

			serializer.Deserialize<Subject>(document)
			          .Should()
			          .BeEquivalentTo(instance);
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}

		sealed class SubjectConverter : ConverterBase<Subject>
		{
			public static SubjectConverter Default { get; } = new SubjectConverter();

			SubjectConverter() {}

			public override Subject Parse(string data) => new Subject {Message = data};

			public override string Format(Subject instance) => instance.Message;
		}
	}
}