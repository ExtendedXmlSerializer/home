using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests
{
	public class IssuesTests
	{
		[Fact]
		public void Issue125()
		{
			var person = new Person { FirstName = "John", LastName = string.Empty, Nationality = "British", Married = true };
			var support = new ConfigurationContainer().ForTesting();
			support.Cycle(person)
				   .ShouldBeEquivalentTo(person);
		}

		public class Person
		{
			public string FirstName { get; set; }

			public string LastName { get; set; }

			public string Nationality { get; set; }

			public bool Married { get; set; }
		}

	}
}
