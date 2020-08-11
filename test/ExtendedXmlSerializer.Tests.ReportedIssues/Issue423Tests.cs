using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
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
			var serializer = new ConfigurationContainer().UseOptimizedNamespaces().Create().ForTesting();
			var first = new Subject
			{
				Message = "Hello World!",
				Dictionaries =
				{
					new Dictionary<int, string> {[123] = "Hello", [456] = "World"},
					new Dictionary<int, string> {[678] = "Hello", [910] = "World"}
				}
			};
			var second = new Subject
			{
				Message = "Hello again!",
				Dictionaries =
				{
					new Dictionary<int, string> {[1112] = "Hello", [1314] = "Again"},
					new Dictionary<int, string> {[1516] = "Hello", [1718] = "Again"}
				}
			};
			var instance = new[] {first, second}.ToList();

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		sealed class Subject
		{
			[UsedImplicitly]
			public string Message { get; set; }

			[UsedImplicitly]
			public List<Dictionary<int, string>> Dictionaries { get; set; } = new List<Dictionary<int, string>>();
		}
	}
}