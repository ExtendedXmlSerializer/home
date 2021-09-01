using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue545Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new ClassWithDictionary();
			instance.Dict.Add("key",123);

			var configuration = new ConfigurationContainer();
			var serializer    = configuration.Create().ForTesting();

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		[Fact]
		public void VerifyImplicitTyping()
		{
			var instance = new ClassWithDictionary();
			instance.Dict.Add("key",123);

			var configuration = new ConfigurationContainer().EnableImplicitTyping(typeof(ClassWithDictionary));
			var serializer    = configuration.Create().ForTesting();

			//serializer.WriteLine(instance);

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		[Fact]
		public void VerifyOptimizedImplicitTyping()
		{
			var instance = new ClassWithDictionary();
			instance.Dict.Add("key",123);

			var configuration = new ConfigurationContainer().UseOptimizedNamespaces().EnableImplicitTyping(typeof(ClassWithDictionary));
			var serializer    = configuration.Create().ForTesting();

			//serializer.WriteLine(instance);

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}


		public class ClassWithDictionary
		{
			public ClassWithDictionary()
			{
				Dict = new Dictionary<string, object>();

				ListWithItems = new List<ClassWithProperty>();
				ListWithItems.Add(new ClassWithProperty());
			}

			public Dictionary<string,object> Dict { get; set; }

			public IList<ClassWithProperty> ListWithItems { get; set; }
		}

		public class ClassWithProperty
		{
			public string Text { get; set; }
		}

	}
}
