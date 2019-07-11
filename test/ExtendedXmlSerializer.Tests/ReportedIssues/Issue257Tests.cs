using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue257Tests
	{
		[Fact]
		void Verify()
		{
			var subject = new ConfigurationContainer().EnableMemberExceptionHandling()
													  .ConfigureType<Subject>()
													  .Member(re => re.Version)
													  .Attribute()
													  .Create()
													  .ForTesting();

			var instance = new Subject { Bar = "Hello", Version = "1" };
			subject.Cycle(instance)
				   .ShouldBeEquivalentTo(instance);
		}

		sealed class Subject
		{
			public string Bar { get; set; }

			[XmlAttribute]
			public string Version { get; set; }
		}
	}
}