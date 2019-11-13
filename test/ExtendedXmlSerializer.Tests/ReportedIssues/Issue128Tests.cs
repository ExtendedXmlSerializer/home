using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.ObjectModel;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue128Tests
	{
		[Fact]
		public void EmptyItems()
		{
			var serializer = new ConfigurationContainer().ForTesting();

			serializer.Serialize(new Subject())
			          .Should()
			          .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue128Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" />");

			var instance = new Subject();
			instance.Counts.Add(6776);
			serializer.Serialize(instance)
			          .Should()
			          .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue128Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Counts><int xmlns=""https://extendedxmlserializer.github.io/system"">6776</int></Counts></Issue128Tests-Subject>");
		}

		[Fact]
		public void ConditionalEmit()
		{
			var serializer = new ConfigurationContainer().Type<Container>()
			                                             .Member(x => x.Dummy)
			                                             .EmitWhen(x => x.Id != 0)
			                                             .Create();

			serializer.Serialize(new Container {Dummy = new Dummy {Id = 6}})
			          .Should()
			          .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue128Tests-Container xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Dummy><Id>6</Id></Dummy></Issue128Tests-Container>");

			serializer.Serialize(new Container {Dummy = new Dummy {Id = 0}})
			          .Should()
			          .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><Issue128Tests-Container xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" />");
		}

		public class Container
		{
			public Dummy Dummy { get; set; }
		}

		public class Dummy
		{
			public int Id { get; set; }
		}

		public class Subject
		{
			// ReSharper disable once CollectionNeverQueried.Global
			public ObservableCollection<int> Counts { get; } = new ObservableCollection<int>();
		}
	}
}