using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.ObjectModel;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue145Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Type<Item>()
			                                             .EmitWhen(x => x?.Enabled ?? false)
			                                             .Create()
			                                             .ForTesting();
			serializer.Cycle(new Subject {Item = new Item {Enabled = true}})
			          .Item.Should()
			          .NotBeNull();

			serializer.Cycle(new Subject {Item = new Item {Enabled = false}})
			          .Item.Should()
			          .BeNull();

			var items = serializer.Cycle(new Collection<Item>
			                             {
				                             new Item {Enabled = true},
				                             new Item {Enabled = false},
				                             new Item {Enabled = true}
			                             });
			items.Should()
			     .HaveCount(2)
			     .And.Subject.Should()
			     .OnlyContain(x => x.Enabled);
		}




		sealed class Subject
		{
			public Item Item { get; set; }
		}

		sealed class Item
		{
			public bool Enabled { get; set; }
		}
	}
}
