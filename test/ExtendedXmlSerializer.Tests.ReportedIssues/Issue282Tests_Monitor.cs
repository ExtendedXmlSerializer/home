using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue282Tests_Monitor
	{
		[Fact]
		void Verify()
		{
			var instances = new List<string>();
			IExtendedXmlSerializer serializer = new ConfigurationContainer().Type<string>()
			                                                                .WithMonitor(new Monitor(instances))
			                                                                .Create();
			const string message = "Hello World!";
			instances.Should().BeEmpty();
			serializer.Serialize(new Subject {Message = message});
			instances.Should().Contain(message);
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}

		sealed class Monitor : ISerializationMonitor<string>
		{
			readonly List<string> _store;

			public Monitor(List<string> store) => _store = store;

			public void OnSerializing(IFormatWriter writer, string instance) {}

			public void OnSerialized(IFormatWriter writer, string instance)
			{
				_store.Add(instance);
			}

			public void OnDeserializing(IFormatReader reader, Type instanceType) {}

			public void OnActivating(IFormatReader reader, Type instanceType) {}

			public void OnActivated(string instance) {}

			public void OnDeserialized(IFormatReader reader, string instance) {}
		}
	}
}