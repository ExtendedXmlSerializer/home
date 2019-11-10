using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue205Tests
	{
		[Fact]
		void Verify()
		{
			var snoopy       = new Person("Snoopy");
			var charlieBrown = new Person("Charles \"Charlie\" Brown");
			var sallyBrown   = new Person("Sally Brown");
			var marcie       = new Person("Marcie");

			snoopy.Friends.Add(charlieBrown);
			snoopy.Friends.Add(sallyBrown);
			snoopy.Friends.Add(marcie);
			snoopy.Friends.Add(charlieBrown); // added twice -- intentional

			var cycled = new ConfigurationContainer()
			             .EnableParameterizedContent()
			             .EnableReferences()
			             .Create()
			             .ForTesting()
			             .Cycle(snoopy);

			snoopy.Should().BeEquivalentTo(cycled);

			cycled.Friends.Should()
			      .HaveCount(4);
			cycled.Friends.First()
			      .Should()
			      .Be(cycled.Friends.Last());
		}

		public class Person
		{
			public Person(string name) : this(name, new List<Person>()) {}

			public Person(string name, IList<Person> friends)
			{
				Name    = name;
				Friends = friends;
			}

			public string Name { [UsedImplicitly] get; }
			public IList<Person> Friends { get; }
		}

		/**/
	}
}