using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;
// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue146Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContent()
			                                             .ConfigureType<Plane>()
			                                             .Member(x => x.Rows).Ignore()
			                                             .Member(x => x.Length).Attribute()
			                                             .Create()
			                                             .ForTesting();
			var subject = new Plane(4.33);
			serializer.Cycle(subject)
			          .ShouldBeEquivalentTo(subject);
			serializer.Serialize(subject)
			          .Should()
			          .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue146Tests-Plane Length=""4.33"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" />");
		}

		public sealed class Plane
		{
			public double Length { get; }

			public int Rows { get; set; }

			public Plane(int rows, double Length)
			{
				Rows = rows;
				this.Length = Length;
			}

			public Plane(double Length) => this.Length = Length;
		}
	}
}