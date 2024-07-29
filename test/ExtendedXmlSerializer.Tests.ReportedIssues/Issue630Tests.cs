using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue630Tests
	{
		[Fact]
		public void Verify()
		{
			var container = new ConfigurationContainer().Create().ForTesting();
			var instance = new Terminate(true);
			container.Cycle(instance).All.Should().BeTrue();
		}

		[Fact]
		public void VerifyPassThroughValue()
		{
			var sut = new ConfigurationContainer().Type<Envelope>().WithMonitor(Monitor.Default).Create().ForTesting();

			var instance = new Envelope { Version = 1.2f, SubObject = new() };
			sut.Cycle(instance).SubObject.Version.Should().Be(1.2f);
		}

		sealed class Monitor : ISerializationMonitor<Envelope>
		{
			public static Monitor Default { get; } = new();

			Monitor() {}

			public void OnSerializing(IFormatWriter writer, Envelope instance) {}

			public void OnSerialized(IFormatWriter writer, Envelope instance) {}

			public void OnDeserializing(IFormatReader reader, Type instanceType) {}

			public void OnActivating(IFormatReader reader, Type instanceType) {}

			public void OnActivated(Envelope instance) {}

			public void OnDeserialized(IFormatReader reader, Envelope instance)
			{
				instance.SubObject.Version = instance.Version;
			}
		}

		sealed class Envelope
		{
			public float Version {get; set;}
			public SubObject SubObject { get; set; }
		}
		sealed class SubObject {
			public string Name {get; set;}
			public float Version {get; set;}
		}


		public sealed class Terminate {
			public Terminate() : this(null) {}

			public Terminate(bool? all) => _all = all;

			[XmlElement(ElementName = "All")]
			private bool? _all = null;
			public bool All {
				get { return _all != null; }
			}
		}

	}
}
