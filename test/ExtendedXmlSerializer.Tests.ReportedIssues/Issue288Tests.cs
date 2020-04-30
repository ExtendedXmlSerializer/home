using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue288Tests
	{
		[Fact]
		void Verify()
		{
			var instance  = new {Test = "Hello World!"};
			var container = new ConfigurationContainer().Create().ForTesting();
			container.Invoking(x => x.Serialize(instance))
			         .Should()
			         .Throw<InvalidOperationException>()
			         .WithMessage("Dynamic/anonymous types are not supported.");
		}
	}
}