using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Encryption;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Xml.Linq;
using XmlWriter = System.Xml.XmlWriter;

// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue132Tests
	{
		public void Examples()
		{
			new ConfigurationContainer().EnableDeferredReferences()
			                            .Type<IElement>().EnableReferences(p => p.Id)
										.Type<Section>().EnableReferences(p => p.Id)
										.Type<Building>().EnableReferences(p => p.Id)
			                            .Create();


			new ConfigurationContainer().Type<Section>()
			                            .Name("person")
			                            .Member(p => p.IsSelected)
			                            .Name("Selected")
			                            .Identity()
			                            .Order(2)
			                            .Member(p => p.IsEmpty)
			                            .Name("Empty")
			                            .Create();


			var serializer = new ConfigurationContainer()

				.Type<Person>() // Configuration of Person class
				.Member(p => p.Password) // First member
				.Name("P")
				.Encrypt()
				.Member(p => p.Name) // Second member
				.Name("T")
				.UseEncryptionAlgorithm(new CustomEncryption())
				.Type<TestClass>() // Configuration of another class
				.CustomSerializer(new TestClassSerializer())
				.Create();


			new ConfigurationContainer()
				.Type<Person>()
				.Member(p => p.Name).Identity()
				.OnlyConfiguredProperties()
				.Create();

			new ConfigurationContainer().Type<Person>()
				.AddMigration(element => {})
				.OnlyConfiguredProperties()
				.Create();

			new ConfigurationContainer().Type<Person>()
				.OnlyConfiguredProperties()
				.AddMigration(element => { })
				.Create();

			new ConfigurationContainer()
				.Type<Person>()
				.Member(p => p.Name).Identity()
				.OnlyConfiguredProperties()
				.Create();

			new ConfigurationContainer()
				.Type<Person>()
				.OnlyConfiguredProperties()
				.Member(p => p.Name).Identity()
				.Create();

			new ConfigurationContainer().Type<Person>()
				.CustomSerializer((writer, person) => {}, element => new Person())
				.EnableReferences(p=>p.Name);

			new ConfigurationContainer().Type<Person>()
				.EnableReferences(p => p.Name)
				.CustomSerializer((writer, person) => { }, element => new Person());

			new ConfigurationContainer().Type<Person>()
				.CustomSerializer((writer, person) => { }, element => new Person())
				.Member(p => p.Name).Identity();

			new ConfigurationContainer().Type<Person>()
				.Member(p => p.Name).Identity()
				.CustomSerializer((writer, person) => { }, element => new Person());

			new ConfigurationContainer().Type<Person>()
				.AddMigration(element => { })
				.Member(p => p.Name).Identity();

			new ConfigurationContainer().Type<Person>()
				.Member(p => p.Name)
				.Identity()
				.AddMigration(element => {});

			var config = new ConfigurationContainer();
			config.EnableDeferredReferences();
			config.Type<Section>().Member(p => p.Id).Name("Identity");
			config.Type<Section>().EnableReferences(p => p.Id);
			var exs = config.Create();

		}

		public class TestClassSerializer : IExtendedXmlCustomSerializer<TestClass>
		{
			public TestClass Deserialize(XElement xElement)
			{
				throw new System.NotImplementedException();
			}

			public void Serializer(XmlWriter xmlWriter, TestClass obj)
			{
				throw new System.NotImplementedException();
			}
		}

		public class Person
		{
			public string Name { get; set; }
			public string Password { get; set; }
		}

		class CustomEncryption : IEncryption
		{
			public string Parse(string data)
			{
				throw new System.NotImplementedException();
			}

			public string Format(string instance)
			{
				throw new System.NotImplementedException();
			}
		}

		public class TestClass
		{

		}

		interface IElement
		{
			string Id { get; }
		}

		sealed class Building : IElement
		{
			public string Id { get; set; }
		}

		sealed class Section : IElement
		{
			public string Id { get; set; }

			public bool IsSelected { get; set; }
			public bool IsEmpty { get; set; }
		}
	}
}
