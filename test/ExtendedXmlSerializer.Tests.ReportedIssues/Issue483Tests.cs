using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue483Tests
	{
		[Fact]
		public void Verify()
		{
			//// Write legacy XML
			//var rootInitialVersion = new Root { Identifier = Guid.NewGuid() };
			//var someClass1 = new SomeClass { Id = 1, SomeString = "String1" };
			//var someClass2 = new SomeClass { Id = 2, SomeString = "String2" };
			//var children = new List<Node>();
			//for (var i = 1; i < 10; i++)
			//	children.Add(new Node
			//	{
			//		Id = i,
			//		Blubb = i % 2 == 0 ? someClass1 : someClass2
			//	});
			//rootInitialVersion.Children = children;

			//var serializer = new ConfigurationContainer()
			//	.ConfigureType<SomeClass>()
			//	.EnableReferences(s => s.Id)
			//	.Create();
			//var initialXml = serializer.Serialize(new XmlWriterSettings { Indent = true }, rootInitialVersion);

			var initialXml = @"<?xml version='1.0' encoding='utf-8'?>
								<Root xmlns='clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues'>
								  <Identifier>391047bb-21a7-46cb-b49c-6b9353305d09</Identifier>
								  <Children>
									<Capacity>16</Capacity>
									<Node>
									  <Id>1</Id>
									  <Blubb Id='2'>
										<SomeString>String2</SomeString>
									  </Blubb>
									</Node>
									<Node>
									  <Id>2</Id>
									  <Blubb Id='1'>
										<SomeString>String1</SomeString>
									  </Blubb>
									</Node>
									<Node>
									  <Id>3</Id>
									  <Blubb xmlns:exs='https://extendedxmlserializer.github.io/v2' exs:entity='2' />
									</Node>
									<Node>
									  <Id>4</Id>
									  <Blubb xmlns:exs='https://extendedxmlserializer.github.io/v2' exs:entity='1' />
									</Node>
									<Node>
									  <Id>5</Id>
									  <Blubb xmlns:exs='https://extendedxmlserializer.github.io/v2' exs:entity='2' />
									</Node>
									<Node>
									  <Id>6</Id>
									  <Blubb xmlns:exs='https://extendedxmlserializer.github.io/v2' exs:entity='1' />
									</Node>
									<Node>
									  <Id>7</Id>
									  <Blubb xmlns:exs='https://extendedxmlserializer.github.io/v2' exs:entity='2' />
									</Node>
									<Node>
									  <Id>8</Id>
									  <Blubb xmlns:exs='https://extendedxmlserializer.github.io/v2' exs:entity='1' />
									</Node>
									<Node>
									  <Id>9</Id>
									  <Blubb xmlns:exs='https://extendedxmlserializer.github.io/v2' exs:entity='2' />
									</Node>
								  </Children>
								</Root>";

			new ConfigurationContainer().Type<SomeClass>()
			                            .EnableReferences(s => s.Id)
			                            .As<ITypeConfiguration>()
			                            .AddMigration(new SomeClassMigrations())
			                            .Create()
			                            .Deserialize<Root>(initialXml)
			                            .Children
			                            .Select(x => x.Blubb.RenamedProperty)
			                            .Take(4)
			                            .Should()
			                            .Equal("String2", "String1", "String2", "String1");
		}
	}
}