using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue340Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Subject { Contents = new[] {"Hello", "World!"}.Select(x => x) };
			new ConfigurationContainer().ForTesting()
			                            .Cycle(instance)
			                            .Should()
			                            .BeEquivalentTo(instance);
		}

		[Fact]
		void VerifyImmutable()
		{
			var instance = new ImmutableSubject(new[] {"Hello", "World!"});
			new ConfigurationContainer().ForTesting()
			                            .Cycle(instance)
			                            .Should()
			                            .BeEquivalentTo(instance);
		}

		sealed class Subject
		{
			public IEnumerable<string> Contents { get; set; }
		}

		sealed class ImmutableSubject
		{
			public ImmutableSubject(IEnumerable<string> contents) => Contents = contents;

			public IEnumerable<string> Contents { get; }
		}
	}
}