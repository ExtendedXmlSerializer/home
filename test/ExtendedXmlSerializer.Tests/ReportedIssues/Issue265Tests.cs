using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue265Tests
	{
		[Fact]
		void Verify()
		{
			const string message = "Hello World!";
			new ConfigurationContainer().Create()
			                            .ForTesting()
			                            .Cycle(new Subject(message))
			                            .Message()
			                            .Should()
			                            .Be(message);
		}

		sealed class Subject
		{
			[XmlElement] string _message;

			public Subject() : this("Default Value") {}

			public Subject(string test) => _message = test;

			public string Message() => _message;
		}
	}
}