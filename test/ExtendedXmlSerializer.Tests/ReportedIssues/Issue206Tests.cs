using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue206Tests
	{
		[Fact]
		void Verify()
		{
			var travelFiles = new[] {new TravelFile()};

			var support = new ConfigurationContainer().EnableImplicitTyping(typeof(TravelFile[]), typeof(TravelFile))
			                                          .Type<TravelFile[]>()
			                                          .Name("ArrayOfTravelFile")
			                                          .Create()
			                                          .ForTesting();

			support.Deserialize<TravelFile[]>(@"<?xml version=""1.0"" encoding=""utf-8""?><ArrayOfTravelFile><Issue206Tests-TravelFile /></ArrayOfTravelFile>")
			       .ShouldBeEquivalentTo(travelFiles);
		}

		sealed class TravelFile {}
	}
}