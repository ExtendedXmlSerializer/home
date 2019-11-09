using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
	public class MigrationsExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var container = new ConfigurationContainer();
			container.Type<Subject>()
			         .AddMigration(new PropertyMigration("OldPropertyName", nameof(Subject.PropertyName)));
			var support  = new SerializationSupport(container);
			var instance = new Subject {PropertyName = "Hello World!"};
			support.Assert(instance,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><MigrationsExtensionTests-Subject xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:version=""1"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Xml;assembly=ExtendedXmlSerializer.Tests""><PropertyName>Hello World!</PropertyName></MigrationsExtensionTests-Subject>");

			var data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><MigrationsExtensionTests-Subject xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Xml;assembly=ExtendedXmlSerializer.Tests""><OldPropertyName>Hello World from Old Property!</OldPropertyName></MigrationsExtensionTests-Subject>";
			var migrated = support.Deserialize<Subject>(data);
			Assert.NotNull(migrated);
			Assert.Equal("Hello World from Old Property!", migrated.PropertyName);
		}

		class Subject
		{
			public string PropertyName { get; set; }
		}
	}
}