using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue260Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Foo {Bar = new Bar()};
			instance.Bar.Foos.Add(instance);

			var serializer = new ConfigurationContainer().EnableReferences()
			                                             .Create();

			var cycled = serializer.Cycle(instance);

			cycled.Should()
			      .BeSameAs(cycled.Bar.Foos.Only());

			cycled.Bar.Should()
			      .BeSameAs(cycled.Bar.Foos.Only()
			                      .Bar);
		}

		public sealed class Foo
		{
			public Bar Bar { get; set; }
		}

		public sealed class Bar
		{
			public ICollection<Foo> Foos { get; set; } = new List<Foo>();
		}
	}
}