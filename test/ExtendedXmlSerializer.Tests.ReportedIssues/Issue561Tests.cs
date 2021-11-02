using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue561Tests
	{
		[Fact]
		public void Verify()
		{
			var subject  = new ConfigurationContainer().EnableImplicitTyping(typeof(double)).Create().ForTesting();
			const double instance = 123d;
			subject.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><double>123</double>");
			subject.Cycle(instance).Should().Be(instance);
		}
	}
}
