using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Content
{
	public class ImplicitlyDefinedDefaultValueAlterationTests
	{
		[Fact]
		public void EmptyElement()
		{
			const string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><ImplicitlyDefinedDefaultValueAlterationTests-Person xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Content;assembly=ExtendedXmlSerializer.Tests""><FirstName>John</FirstName><LastName></LastName><Nationality>British</Nationality><Married></Married></ImplicitlyDefinedDefaultValueAlterationTests-Person>";

			var serializer = new ConfigurationContainer().EnableImplicitlyDefinedDefaultValues()
			                                             .ForTesting();

			var person = serializer.Deserialize<Person>(data);

			Assert.False(person.Married);
		}

		[Fact]
		public void EmptyValue()
		{
			const string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><ImplicitlyDefinedDefaultValueAlterationTests-Person xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Content;assembly=ExtendedXmlSerializer.Tests""><FirstName>John</FirstName><LastName></LastName><Nationality>British</Nationality><Married/></ImplicitlyDefinedDefaultValueAlterationTests-Person>";

			var serializer = new ConfigurationContainer().EnableImplicitlyDefinedDefaultValues()
			                                             .ForTesting();

			var person = serializer.Deserialize<Person>(data);

			Assert.False(person.Married);
		}

		[Fact]
		public void EmptyElementCount()
		{
			const string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><ImplicitlyDefinedDefaultValueAlterationTests-Person xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Content;assembly=ExtendedXmlSerializer.Tests""><FirstName>John</FirstName><LastName></LastName><Nationality>British</Nationality><Count></Count></ImplicitlyDefinedDefaultValueAlterationTests-Person>";

			var serializer = new ConfigurationContainer().EnableImplicitlyDefinedDefaultValues()
			                                             .ForTesting();

			var person = serializer.Deserialize<Person>(data);

			Assert.Equal(0, person.Count);
		}

		[Fact]
		public void EmptyValueCount()
		{
			const string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><ImplicitlyDefinedDefaultValueAlterationTests-Person xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Content;assembly=ExtendedXmlSerializer.Tests""><FirstName>John</FirstName><LastName></LastName><Nationality>British</Nationality><Count/></ImplicitlyDefinedDefaultValueAlterationTests-Person>";

			var serializer = new ConfigurationContainer().EnableImplicitlyDefinedDefaultValues()
			                                             .ForTesting();

			var person = serializer.Deserialize<Person>(data);

			Assert.Equal(0, person.Count);
		}

		public class Person
		{
			public string FirstName { get; set; }

			public string LastName { get; set; }

			public string Nationality { get; set; }

			public bool Married { get; [UsedImplicitly] set; }

			public int Count { get; [UsedImplicitly] set; }
		}
	}
}