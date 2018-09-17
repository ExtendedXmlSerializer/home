using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue205Tests
	{
		[Fact]
		void Verify()
		{
			/*var subject = new Subject();
			var list = new List<Subject>{ subject, subject };*/

			/*var cycled = new ConfigurationContainer()

							//						 .EnableDeferredReferences()
			             .EnableReferences()
			             .EnableParameterizedContent()

			                                         .Create()
			                                         .ForTesting()
			                                         .Cycle(list);

			cycled.ShouldBeEquivalentTo(list);*/

			/*var charlieBrown = new Person(@"Charles ""Charlie"" Brown");
			var snoopy       = new Person("Snoopy", new List<Person>{ charlieBrown, charlieBrown });*/

			/*var charlieBrown = new Person();
			var snoopy       = new Person(new List<Person>{ charlieBrown, charlieBrown });


			                                         /*.Cycle(snoopy)#1#;



			snoopy.ShouldBeEquivalentTo(cycled);

			cycled.Friends.Should()
			      .HaveCount(4);
			cycled.Friends.First()
			      .Should()
			      .Be(cycled.Friends.Last());*/
		}

		[Fact]
		void VerifyList()
		{

			var subject = new Subject(new List<int>());

			var cycled = new ConfigurationContainer()

			             .EnableParameterizedContent()
			             .EnableReferences()
			             .Create()
			             .ForTesting()
			             .Cycle(subject);

			cycled.ShouldBeEquivalentTo(subject);
			cycled.Numbers.Should()
			      .NotBeNull();
		}

		sealed class Subject
		{
			public Subject(IList<int> numbers) => Numbers = numbers;

			public IList<int> Numbers { get; }
		}
	}

	/*public class Person
	{
		public Person() : this(new List<Person>()) {}

		public Person(IList<Person> friends) => Friends = friends;

		public IList<Person> Friends { get; }
	}*/

	/*public class Person
	{
		public Person(string name) : this(name, new List<Person>()) {}

		public Person(string name, IList<Person> friends)
		{
			Name    = name;
			Friends = friends;
		}

		public string Name { get; }

		public IList<Person> Friends { get; }
	}*/
}