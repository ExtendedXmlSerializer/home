using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
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
			                                          .Type<Subject>()
			                                          .Member(re => re.Version)
			                                          .Attribute()
			                                          .Create()
			                                          .ForTesting();

			var instance = new Subject {Bar = "Hello", Version = "1"};
			subject.Cycle(instance)
			       .Should()
			       .BeEquivalentTo(instance);
		}

		sealed class Subject
		{
			public string Bar { [UsedImplicitly] get; set; }

			[XmlAttribute]
			public string Version { get; set; }
		}
	}
}