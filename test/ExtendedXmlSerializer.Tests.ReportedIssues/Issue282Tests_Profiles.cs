using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using Xunit;
// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue282Tests_Profiles
	{
		[Fact]
		void Verify()
		{
			var container = ConfiguredContainer.New<SimpleProfile>();

			var serializer = container.UseAutoFormatting()
			                          .EnableImplicitTyping(typeof(Subject))
			                          .UseOptimizedNamespaces()
			                          .Create();
			var instance = new Subject {Message = "Hello World!"};
			var document = serializer.Serialize(instance);

			document.Should()
			        .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue282Tests_Profiles-Subject Number=""0"" NewMessage=""Hello World!"" />");
		}

		sealed class Subject
		{
			public int Number { get; set; }

			public string Message { get; set; }
		}

		sealed class SimpleProfile : IConfigurationProfile
		{
			public static SimpleProfile Default { get; } = new SimpleProfile();

			SimpleProfile() {}

			public IConfigurationContainer Get(IConfigurationContainer parameter) => parameter.Type<Subject>()
			                                                                                  .Member(x => x.Message)
			                                                                                  .Name("NewMessage");
		}

		[Fact]
		void VerifyComposite()
		{
			var container = ConfiguredContainer.New<ComplexProfile>();

			var serializer = container.UseAutoFormatting()
			                          .EnableImplicitTyping(typeof(Subject))
			                          .UseOptimizedNamespaces()
			                          .Create();
			var instance = new Subject {Message = "Hello World!", Number = 123};
			var document = serializer.Serialize(instance);

			document.Should()
			        .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue282Tests_Profiles-Subject NewNumber=""123"" NewMessage=""Hello World!"" />");
		}

	sealed class ComplexProfile : CompositeConfigurationProfile
	{
		public static ComplexProfile Default { get; } = new ComplexProfile();

		ComplexProfile() : base(SimpleProfile.Default, NumberProfile.Default) {}
	}

	sealed class NumberProfile : IConfigurationProfile
	{
		public static NumberProfile Default { get; } = new NumberProfile();

		NumberProfile() {}

		public IConfigurationContainer Get(IConfigurationContainer parameter) => parameter.Type<Subject>()
		                                                                                  .Member(x => x.Number)
		                                                                                  .Name("NewNumber");
	}
	}
}