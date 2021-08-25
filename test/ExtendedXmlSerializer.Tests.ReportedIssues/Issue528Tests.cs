using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue528Tests
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
