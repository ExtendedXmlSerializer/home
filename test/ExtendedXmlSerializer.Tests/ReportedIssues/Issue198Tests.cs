using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue198Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer()
			                 .Register<Version>(VersionConverter.Default)
			                 .Create()
			                 .ForTesting();
			var version  = new Version(1, 2);
			var instance = new Subject {Version = version};
			var subject  = serializer.Cycle(instance);
			subject.ShouldBeEquivalentTo(instance);
			version.ToString()
			       .Should()
			       .Be(subject.Version.ToString());
		}

		sealed class Subject
		{
			public Version Version { get; set; }
		}
	}
}