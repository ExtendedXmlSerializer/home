using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue203Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Person("I AM MY FATHER!");
			instance.Father = instance;
			new ConfigurationContainer().EnableReferences()
			                            .EnableParameterizedContent()
			                            .Create()
			                            .ForTesting()
			                            .Assert(instance,
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue203Tests-Person xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:identity=""1"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Name>I AM MY FATHER!</Name><Father exs:reference=""1"" /></Issue203Tests-Person>");
		}

		public class Person
		{
			public Person(string name) => Name = name;

			public string Name { [UsedImplicitly] get; }

			public Person Mother { get; set; }

			public Person Father { [UsedImplicitly] get; set; }
		}
	}
}