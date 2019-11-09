using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue143Tests
	{
		[Fact]
		public void Assigned()
		{
			var sut = new ConfigurationContainer().EnableParameterizedContent()
			                                      .Create()
			                                      .ForTesting();

			const string content =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue143Tests-Subject Name=""Testing"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" Factor="".5"" />";

			var subject = sut.Deserialize<Subject>(content);
			subject.Factor.Should()
			       .Be(.5);
		}

		[Fact]
		public void NotAssigned()
		{
			var sut = new ConfigurationContainer().EnableParameterizedContent()
			                                      .Create()
			                                      .ForTesting();

			const string content =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue143Tests-Subject Name=""Testing"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" />";

			var subject = sut.Deserialize<Subject>(content);
			subject.Factor.Should()
			       .Be(.95);
		}

		sealed class Subject
		{
			public Subject(string name, double factor = .95)
			{
				Name   = name;
				Factor = factor;
			}

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public string Name { get; }
			public double Factor { get; }
		}
	}
}