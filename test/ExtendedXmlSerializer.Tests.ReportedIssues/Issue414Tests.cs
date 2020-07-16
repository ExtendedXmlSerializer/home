using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue414Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Type<Base>()
			                                             .AddMigration(EmptyMigration.Default)
			                                             .Create()
			                                             .ForTesting();
			var container = new Container {Content = new Inherit()};
			serializer.Cycle(container).Should().BeEquivalentTo(container);
		}

		public class Container
		{
			public Base Content { get; set; }
		}

		public class Inherit : Base {}

		public class Base {}

		sealed class EmptyMigration : IEnumerable<Action<XElement>>
		{
			public static EmptyMigration Default { get; } = new EmptyMigration();

			EmptyMigration() {}

			public IEnumerator<Action<XElement>> GetEnumerator()
			{
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}