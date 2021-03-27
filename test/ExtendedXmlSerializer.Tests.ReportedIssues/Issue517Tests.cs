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

		[Fact]
		public void VerifyAdditional()
		{
			var container = new Shared.Issue517.ContainerNameSpace.Container();
			for (int i = 0; i < 1; i++)
			{
				container.Childrens.Add(new Shared.Issue517.ItemNameSpace.Item());
			}

			IConfigurationContainer config = new ConfigurationContainer();
			config.Type<Shared.Issue517.ContainerNameSpace.Container>().AddMigration(new EmptyMigration());
			var serializer = config.Create().ForTesting();

			serializer.Cycle(container).Should().BeEquivalentTo(container);

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