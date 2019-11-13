using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Content.Members
{
	public class AllowedMembersExtensionTests
	{
		[Fact]
		public void Ignore()
		{
			var configuration = new ConfigurationContainer();
			configuration.Type<Subject>()
			             .Member(x => x.Property2)
			             .Ignore();
			var support  = new SerializationSupport(configuration);
			var instance = new Subject {Property1 = "Hello World!", Property2 = 1000, Property3 = DateTime.Now};
			var actual   = support.Cycle(instance);
			Assert.NotEqual(instance.Property2, actual.Property2);
		}

		[Fact]
		public void Include()
		{
			var serializer = new ConfigurationContainer();
			serializer.Type<Subject>()
			          .Member(x => x.Property2)
			          .Include();
			var support  = new SerializationSupport(serializer);
			var instance = new Subject {Property1 = "Hello World!", Property2 = 1000, Property3 = DateTime.Now};
			var actual   = support.Cycle(instance);
			Assert.NotEqual(instance.Property1, actual.Property1);
			Assert.Equal(instance.Property2, actual.Property2);
			Assert.NotEqual(instance.Property3, actual.Property3);
		}

		[Fact]
		public void TypedOnlyConfiguredProperties()
		{
			var container = new ConfigurationContainer();
			var type      = container.Type<Subject>();
			type.Member(x => x.Property2);
			type.Member(x => x.Property1);
			type.IncludeConfiguredMembers();
			var support  = new SerializationSupport(container);
			var instance = new Subject {Property1 = "Hello World!", Property2 = 1000, Property3 = DateTime.Now};
			var actual   = support.Cycle(instance);
			Assert.Equal(instance.Property1, actual.Property1);
			Assert.Equal(instance.Property2, actual.Property2);
			Assert.NotEqual(instance.Property3, actual.Property3);
		}

		[Fact]
		public void GlobalOnlyConfiguredProperties()
		{
			var container = new ConfigurationContainer();
			var type      = container.Type<Subject>();
			type.Member(x => x.Property2);
			type.Member(x => x.Property3);
			type.IncludeConfiguredMembers();
			var support  = new SerializationSupport(container);
			var instance = new Subject {Property1 = "Hello World!", Property2 = 1000, Property3 = DateTime.Now};
			var actual   = support.Cycle(instance);
			Assert.NotEqual(instance.Property1, actual.Property1);
			Assert.Equal(instance.Property2, actual.Property2);
			Assert.Equal(instance.Property3, actual.Property3);
		}

		class Subject
		{
			public string Property1 { get; set; }

			public int Property2 { get; set; }

			public DateTime Property3 { get; set; }
		}
	}
}