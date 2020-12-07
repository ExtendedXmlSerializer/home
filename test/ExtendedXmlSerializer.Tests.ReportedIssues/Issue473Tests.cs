using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue473Tests
	{
		[Fact]
		public void Verify()
		{
			// language=XML
			const string input =
				@"<Zoo><Animals><Issue473Tests-Animal Name=""Hello""/><Human/><Issue473Tests-Animal Name=""World""/></Animals></Zoo>";

			var serializer = new ConfigurationContainer().Type<Zoo>()
			                                             .Name("Zoo")
			                                             .EnableImplicitTyping(typeof(Zoo), typeof(Animal))
			                                             .WithUnknownContent()
			                                             .Continue()
			                                             .Create();

			var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
			var reader        = XmlReader.Create(contentStream);
			var deserialized  = (Zoo)serializer.Deserialize(reader);

			deserialized.Animals.Count.Should().Be(2);

			var expected = new Zoo
				{ Animals = new List<Animal> { new Animal { Name = "Hello" }, new Animal { Name = "World" } } };
			deserialized.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void VerifyThrow()
		{
			// language=XML
			const string input =
				@"<Zoo><Animals><Issue473Tests-Animal Name=""Hello""/><Human/><Issue473Tests-Animal Name=""World""/></Animals></Zoo>";

			var serializer = new ConfigurationContainer().Type<Zoo>()
			                                             .Name("Zoo")
			                                             .EnableImplicitTyping(typeof(Zoo), typeof(Animal))
			                                             .WithUnknownContent()
			                                             .Throw()
			                                             .Create();

			serializer.Invoking(x => x.Deserialize<Zoo>(input))
			          .Should()
			          .Throw<XmlException>()
			          .WithMessage("Unknown/invalid member encountered: 'Human'. Line 1, position 52.");
		}

		public class Zoo
		{
			public List<Animal> Animals { get; set; }
		}

		public class Animal
		{
			public string Name { get; set; }
		}
	}
}