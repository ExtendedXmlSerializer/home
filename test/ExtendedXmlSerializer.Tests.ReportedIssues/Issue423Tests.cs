using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue423Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Create().ForTesting();
			var first = new Subject{ Dictionaries = {new Dictionary<int, string>{[123] = "Hello", [456] = "World"}} };
			var second = new Subject{ Dictionaries = {new Dictionary<int, string>{[789] = "Hello", [101112] = "Again"}} };
			var instance = new[] {first, second};

			serializer.Cycle(instance).Should().BeEquivalentTo(instance.Cast<Subject>());
		}

		sealed class Subject
		{
			public List<Dictionary<int, string>> Dictionaries = new List<Dictionary<int, string>>();
		}
	}
}
