using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue178Tests
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

	public class SomeClassMigrations : IEnumerable<Action<XElement>>
	{
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<Action<XElement>> GetEnumerator()
		{
			yield return MigrationV0;
		}

		public static void MigrationV0(XElement node)
		{
			var someStringElement = node.Member("SomeString");
			if (someStringElement != null)
			{
				// Add new node
				node.Add(new XElement("RenamedProperty"), someStringElement.Value);
				// Remove old node
				someStringElement.Remove();
			}
		}
	}

	public class Root
	{
		public Guid Identifier { get; set; }

		// ReSharper disable once CollectionNeverUpdated.Global
		public List<Node> Children { get; [UsedImplicitly] set; }
	}

	public class Node
	{
		public int Id { get; set; }
		public SomeClass Blubb { get; [UsedImplicitly] set; }
	}

	public class SomeClass
	{
		public int Id { get; [UsedImplicitly] set; }

		public string RenamedProperty { get; set; }
	}
}