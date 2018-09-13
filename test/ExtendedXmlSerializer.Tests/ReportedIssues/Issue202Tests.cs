using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue202Tests
	{
		[Fact]
		void FromIdentity()
		{
			var subject = new Subject();
			subject.Subject1 = subject;
			new ConfigurationContainer().EnableReferences().Create().ForTesting().Assert(subject, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue202Tests-Subject xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:identity=""1"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Subject1 exs:reference=""1"" /></Issue202Tests-Subject>");
		}

		[Fact]
		void Declared()
		{
			var subject = new Subject {Name = "My name, yo."};
			subject.Subject1 = subject;
			new ConfigurationContainer().Type<Subject>().EnableReferences(x => x.Name).Create().ForTesting().Assert(subject, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue202Tests-Subject Name=""My name, yo."" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Subject1 xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:entity=""My name, yo."" /></Issue202Tests-Subject>");
		}

		public class Subject
		{
			public string Name { get; set; }

			public Subject Subject1 { get; set; }
		}
	}
}
