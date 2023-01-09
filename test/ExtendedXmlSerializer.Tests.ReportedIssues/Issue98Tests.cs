using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue98Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Foo {Bar = new Bar()};
			instance.Bar.Foos.Add(instance);

			var serializer = new ConfigurationContainer().EnableReferences().Create().ForTesting();

			var cycled = serializer.Cycle(instance);

			cycled.Should().BeSameAs(cycled.Bar.Foos.Only());

			cycled.Bar.Should().BeSameAs(cycled.Bar.Foos.Only().Bar);
		}

		[Fact]
		void VerifyList()
		{
			var instance = new Foo {Bar = new Bar()};
			instance.Bar.Foos.Add(instance);

			var serializer = new ConfigurationContainer().EnableReferences()
			                                             .Create();

			var list = new List<Foo> {instance};

			var cycled = serializer.Cycle(list)
			                       .Only();

			cycled.Should()
			      .BeSameAs(cycled.Bar.Foos.Only());

			cycled.Bar.Should()
			      .BeSameAs(cycled.Bar.Foos.Only()
			                      .Bar);
		}

		public class Foo
		{
			public virtual Bar Bar { get; set; }
		}

		public class Bar
		{
			public virtual ICollection<Foo> Foos { get; set; } = new List<Foo>();
		}
	}
}