using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue386Tests
	{
		[Fact]
		void Verify()
		{
			var instance = LogMessageSeverity.Debug | LogMessageSeverity.Informational;
			var serializer = new ConfigurationContainer().ForTesting();
			serializer.Cycle(instance).Should().Be(instance);
		}

		[Flags]
		public enum LogMessageSeverity
		{
			Debug         = 1,
			Informational = 2,
			Critical      = 4
		}
	}
}