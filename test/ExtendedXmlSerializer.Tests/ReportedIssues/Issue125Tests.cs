using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue125Tests
	{
		[Fact]
		public void Issue125()
		{
			var person = new Person { FirstName = "John", LastName = string.Empty, Nationality = "British", Married = true };
			var support = new ConfigurationContainer().ForTesting();
			support.Cycle(person)
				   .Should().BeEquivalentTo(person);
		}

		[Fact]
		public void DefaultValues()
		{
			const string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><IssuesTests-Person xmlns=""clr-namespace:ExtendedXmlSerializer.Tests;assembly=ExtendedXmlSerializer.Tests""><FirstName>John</FirstName><LastName></LastName><Nationality>British</Nationality><Married></Married></IssuesTests-Person>",
				empty =
				@"<?xml version=""1.0"" encoding=""utf-8""?><IssuesTests-Person xmlns=""clr-namespace:ExtendedXmlSerializer.Tests;assembly=ExtendedXmlSerializer.Tests""><FirstName>John</FirstName><LastName></LastName><Nationality>British</Nationality><Married/></IssuesTests-Person>",
				classicEmpty =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><FirstName>John</FirstName><LastName /><Nationality>British</Nationality><Married></Married></Person>",
			    classic =
				             @"<?xml version=""1.0"" encoding=""utf-8""?><Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><FirstName>John</FirstName><LastName /><Nationality>British</Nationality><Married /></Person>";

			var serializer = new ConfigurationContainer().ForTesting();

			data.Invoking(x => serializer.Deserialize<Person>(x))
			    .Should().Throw<InvalidOperationException>();
			empty.Invoking(x => serializer.Deserialize<Person>(x))
			     .Should().Throw<InvalidOperationException>();

			classic.Invoking(x => ClassicSerialization<Person>.Default.Get(x))
			       .Should().Throw<InvalidOperationException>();

			classicEmpty.Invoking(x => ClassicSerialization<Person>.Default.Get(x))
			            .Should().Throw<InvalidOperationException>();
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
