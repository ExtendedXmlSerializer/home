using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue216Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Foo {Baz = {"hello"}};
			var cycle = new ConfigurationContainer().Create()
			                                        .Cycle(instance);
			cycle.ShouldBeEquivalentTo(instance);
		}

		public class Foo
		{
			public List<string> Baz { get; } = new List<string>();
		}
	}
}
