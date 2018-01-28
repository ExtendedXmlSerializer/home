using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue148Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = ConfiguredContainer.New<Profile>()
			                                    .Create()
			                                    .ForTesting();

			var subject = new Subject{ Message = "Hello World!" };
			serializer.Assert(subject, @"<?xml version=""1.0"" encoding=""utf-8""?><ConfiguredSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message></ConfiguredSubject>")
			          .ShouldBeEquivalentTo(subject);
		}


		sealed class Subject
		{
			public string Message { [UsedImplicitly]get; set; }
		}


		sealed class Profile : IConfigurationProfile
		{
			[UsedImplicitly]
			public static Profile Default { get; } = new Profile();
			Profile() {}

			public IConfigurationContainer Get(IConfigurationContainer parameter) => parameter.Type<Subject>()
			                                                                                  .Name("ConfiguredSubject");
		}
	}
}
