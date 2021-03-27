using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Shared.Issue517;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue517Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new Class1 { obj = new Class2() };

			var sut = new ConfigurationContainer().Type<Class1>()
			                                      .AddMigration(new EmptyMigration())
			                                      .Create()
			                                      .ForTesting();

			sut.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		public class EmptyMigration : IEnumerable<Action<XElement>>
		{
			public IEnumerator<Action<XElement>> GetEnumerator()
			{
				yield return x => {}; //Do nothing
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		class Class2 : Interface1 {}
	}
}