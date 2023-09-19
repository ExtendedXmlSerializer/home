using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue604Tests
	{
		[Fact]
		public void Verify()
		{
			var enumerable = new InspectedPropertyTypes<Subject>().Concat(new[] { typeof(DictionaryEntry) });
			var subject = new ConfigurationContainer().Type<DictionaryEntry>()
			                                          .Name("Entry")
			                                          .EnableImplicitTyping(enumerable)
			                                          .Create()
			                                          .ForTesting();
			var instance = new Subject { Store = new() { { "Hello", true }, { "World", false } } };
			subject.Assert(instance,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><Issue604Tests-Subject><Store><Entry><Key>Hello</Key><Value>true</Value></Entry><Entry><Key>World</Key><Value>false</Value></Entry></Store></Issue604Tests-Subject>")
			       .Should()
			       .BeEquivalentTo(instance);
		}

		public sealed class Subject
		{
			public Dictionary<string, bool> Store { get; set; }
		}
	}
}