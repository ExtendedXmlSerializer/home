using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
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

			snoopy.ShouldBeEquivalentTo(cycled);

			cycled.Friends.Should()
			      .HaveCount(4);
			cycled.Friends.First()
			      .Should()
			      .Be(cycled.Friends.Last());

			/*var charlieBrown = new Person();
			var snoopy       = new Person(new List<Person> {charlieBrown, charlieBrown});
			var cycled = new ConfigurationContainer()
			             .EnableParameterizedContent()
			             .EnableReferences()
			             .Create()
			             .ForTesting()
			             .Cycle(snoopy);


			cycled.Friends.First()
			      .Should()
			      .BeSameAs(cycled.Friends.Last());*/
			/*cycled.Should()
			      .ShouldBeEquivalentTo(snoopy)*/;
		}

		public class Person
		{
			public Person(string name) : this(name, new List<Person>()) {}

			public Person(string name, IList<Person> friends)
			{
				Name = name;
				Friends = friends;
			}

			public string Name { get; }
			public IList<Person> Friends { get; }
		}

		/**/


	}
}