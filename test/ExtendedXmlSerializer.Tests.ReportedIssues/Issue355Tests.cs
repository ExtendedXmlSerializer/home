using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue355Tests
	{
		[Fact]
		void Verify()
		{
			var container = ConfiguredContainer.New<CombinedProfile>();
			var serializer = container.UseOptimizedNamespaces()
			                          .EnableParameterizedContent()
			                          .EnableImplicitTyping(typeof(Outer))
			                          //

			                          .Create()
			                          .ForTesting();

			serializer.Assert(new Outer("A", "B", "C"), @"<?xml version=""1.0"" encoding=""utf-8""?><OuterDataThings><Cc>C</Cc><Bb>B</Bb><Aa>A</Aa></OuterDataThings>");
		}

		public class Outer
		{
			public Outer(string a, string b, string c)
			{
				A = a;
				B = b;
				C = c;
			}

			public string C { get; }

			public string B { get; }

			public string A { get; }
		}

		sealed class CombinedProfile : CompositeConfigurationProfile
		{
			public static CombinedProfile Default { get; } = new CombinedProfile();

			CombinedProfile() : base(OuterProfile.Default) {}
		}

		sealed class OuterProfile : IConfigurationProfile
		{
			public static OuterProfile Default { get; } = new OuterProfile();

			OuterProfile() {}

			public IConfigurationContainer Get(IConfigurationContainer parameter)
				=> parameter.Type<Outer>()
				            .Name("OuterDataThings")
							//
				            .Member(x => x.C)
				            .Name("Cc")
				            .Order(0)
							//
				            .Member(x => x.B)
				            .Name("Bb")
				            .Order(1)
							//
				            .Member(x => x.A)
				            .Name("Aa")
				            .Order(2);
		}
	}
}