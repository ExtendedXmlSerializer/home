using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue377Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().Type<Data1>()
			                                             .WithMonitor(Monitor.Default)
			                                             .Create()
			                                             .ForTesting();
			var instance = new Data1();
			instance.MyCollection.Clear();
			instance.MyCollection.Add("data4");
			instance.MyCollection.Add("data5");
			instance.MyCollection.Add("data6");
			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		sealed class Monitor : ISerializationMonitor<Data1>
		{
			public static Monitor Default { get; } = new Monitor();

			Monitor() {}

			public void OnSerializing(IFormatWriter writer, Data1 instance) {}

			public void OnSerialized(IFormatWriter writer, Data1 instance) {}

			public void OnDeserializing(IFormatReader reader, Type instanceType) {}

			public void OnActivating(IFormatReader reader, Type instanceType) {}

			public void OnActivated(Data1 instance)
			{
				instance.MyCollection.Clear();
			}

			public void OnDeserialized(IFormatReader reader, Data1 instance) {}
		}

		sealed class Data1
		{
			public Data1() => MyCollection = new List<string> {"data1", "data2", "data3"};

			public List<string> MyCollection { get; }
		}
	}
}