using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue282Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().Type<Subject>()
			                                             .Register()
			                                             .Serializer()
			                                             .Using(SubjectSerializer.Default)
			                                             .Create()
			                                             .ForTesting();

			var instance = new Subject("Hello World!", 123);
			serializer.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue282Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues"">Hello World!|123</Issue282Tests-Subject>");
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		public sealed class SubjectSerializer : ISerializer<Subject>
		{
			public static SubjectSerializer Default { get; } = new SubjectSerializer();

			SubjectSerializer() {}

			public Subject Get(IFormatReader parameter)
			{
				var parts  = parameter.Content().Split('|');
				var result = new Subject(parts[0], int.Parse(parts[1]));
				return result;
			}

			public void Write(IFormatWriter writer, Subject instance)
			{
				writer.Content($"{instance.Text}|{instance.Number}");
			}
		}

		public class Subject
		{
			public Subject(string text, int number)
			{
				Text   = text;
				Number = number;
			}

			public string Text { get; }
			public int Number { get; }
		}
	}
}