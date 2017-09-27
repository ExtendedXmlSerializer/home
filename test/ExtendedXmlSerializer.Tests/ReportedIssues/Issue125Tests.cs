using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Xunit;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue125Tests
	{
		[Fact]
		public void Issue125()
		{
			var person = new Person { FirstName = "John", LastName = string.Empty, Nationality = "British", Married = true };
			var support = new ConfigurationContainer().ForTesting();
			support.Cycle(person)
				   .ShouldBeEquivalentTo(person);
		}

		[Fact]
		public void DefaultValues()
		{
			const string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><IssuesTests-Person xmlns=""clr-namespace:ExtendedXmlSerializer.Tests;assembly=ExtendedXmlSerializer.Tests""><FirstName>John</FirstName><LastName></LastName><Nationality>British</Nationality><Married/></IssuesTests-Person>",
				empty =
				@"<?xml version=""1.0"" encoding=""utf-8""?><IssuesTests-Person xmlns=""clr-namespace:ExtendedXmlSerializer.Tests;assembly=ExtendedXmlSerializer.Tests""><FirstName>John</FirstName><LastName></LastName><Nationality>British</Nationality><Married/></IssuesTests-Person>",
				classicEmpty =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><FirstName>John</FirstName><LastName /><Nationality>British</Nationality><Married></Married></Person>",
			    classic =
				             @"<?xml version=""1.0"" encoding=""utf-8""?><Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><FirstName>John</FirstName><LastName /><Nationality>British</Nationality><Married /></Person>";


			data.Invoking(x => Deserialize<Person>(x))
			    .ShouldThrow<InvalidOperationException>();
			empty.Invoking(x => Deserialize<Person>(x))
			     .ShouldThrow<InvalidOperationException>();

			classic.Invoking(x => Deserialize<Person>(x))
			       .ShouldThrow<InvalidOperationException>();

			classicEmpty.Invoking(x => Deserialize<Person>(x))
			            .ShouldThrow<InvalidOperationException>();
		}

		T Deserialize<T>(string data)

		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				using (var reader = new XmlReaderFactory().Get(stream))
				{
					var result = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
					return result;
				}
			}
		}

		string Serialize<T>(T instance)
		{
			var serializer = new XmlSerializer(typeof(T));
			using (var stream = new MemoryStream())
			{
				using (var writer = XmlWriter.Create(stream))
				{
					serializer.Serialize(writer, instance);
					writer.Flush();
					stream.Seek(0, SeekOrigin.Begin);
					var result = new StreamReader(stream).ReadToEnd();
					return result;
				}
			}
		}

		public class Person
		{
			public string FirstName { get; set; }

			public string LastName { get; set; }

			public string Nationality { get; set; }

			public bool Married { get; set; }
		}

	}
}
