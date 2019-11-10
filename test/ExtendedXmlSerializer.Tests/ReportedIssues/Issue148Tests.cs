using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
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

			var subject = new Subject {Message = "Hello World!"};
			serializer.Assert(subject,
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><ConfiguredSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message></ConfiguredSubject>")
			          .Should().BeEquivalentTo(subject);
		}

		[Fact]
		public void VerifyComposite()
		{
			var serializer = ConfiguredContainer.New<Combined>()
			                                    .Create()
			                                    .ForTesting();

			var subject = new Subject {Message = "Hello World!"};
			serializer.Assert(subject,
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><ConfiguredSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" />")
			          .Message.Should()
			          .BeNull();
		}

		[Fact]
		public void VerifySerializer()
		{
			var serializer = new Serializer().ForTesting();

			var subject = new Subject {Message = "Hello World!"};
			serializer.Assert(subject,
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><ConfiguredSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message></ConfiguredSubject>")
			          .Should().BeEquivalentTo(subject);
		}

		sealed class Subject
		{
			public string Message { [UsedImplicitly] get; set; }
		}

		sealed class Serializer : ConfiguredSerializer<Profile> {}

		sealed class Profile : IConfigurationProfile
		{
			[UsedImplicitly]
			public static Profile Default { get; } = new Profile();

			Profile() {}

			public IConfigurationContainer Get(IConfigurationContainer parameter) => parameter.Type<Subject>()
			                                                                                  .Name("ConfiguredSubject");
		}

		sealed class Ignore : IConfigurationProfile
		{
			[UsedImplicitly]
			public static Ignore Default { get; } = new Ignore();

			Ignore() {}

			public IConfigurationContainer Get(IConfigurationContainer parameter) => parameter.Type<Subject>()
			                                                                                  .Member(x => x.Message)
			                                                                                  .Ignore();
		}

		sealed class Combined : CompositeConfigurationProfile
		{
			[UsedImplicitly]
			public static Combined Default { get; } = new Combined();

			Combined() : base(Profile.Default, Ignore.Default) {}
		}
	}
}