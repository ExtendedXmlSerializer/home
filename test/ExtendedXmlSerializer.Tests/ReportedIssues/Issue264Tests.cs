using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue264Tests
	{
		[Fact]
		void Verify()
		{
			new ConfigurationContainer().Type<string>()
			                            .RegisterContentComposition(x => new Serializer(x))
			                            .Create()
			                            .Cycle("Hello World!")
			                            .Should()
			                            .Be("- Before Deserialization -Serialized: Hello World!- After Deserialization -");
		}

		[Fact]
		void VerifyMonitorSerialization()
		{
			var list = new List<(SerializationStages, object)>();
			var serializer = new ConfigurationContainer().WithMonitor(new SerializationMonitor(list))
			                                             .Create();
			list.Should()
			    .BeEmpty();
			var instance = new Subject {Message = "Hello World!"};
			var cycled   = serializer.Cycle(instance);
			cycled.ShouldBeEquivalentTo(instance);
			list.Select(x => x.Item1)
			    .Should()
			    .Equal(SerializationStages.OnSerializing, SerializationStages.OnSerializing,
			           SerializationStages.OnSerialized, SerializationStages.OnSerialized);

			list.Select(x => x.Item2)
			    .Should()
			    .Equal(instance, instance.Message, instance.Message, instance);
		}

		[Fact]
		void VerifyMonitorDeserialization()
		{
			var list = new List<(DeserializationStages, object)>();
			var serializer = new ConfigurationContainer().WithMonitor(new DeserializationMonitor(list))
			                                             .Create();
			list.Should()
			    .BeEmpty();
			var instance = new Subject {Message = "Hello World!"};
			var cycled   = serializer.Cycle(instance);
			cycled.ShouldBeEquivalentTo(instance);
			list.Select(x => x.Item1)
			    .Should()
			    .Equal(DeserializationStages.OnActivating, DeserializationStages.OnActivated,
			           DeserializationStages.OnDeserialized, DeserializationStages.OnDeserialized);
			list.Select(x => x.Item2)
			    .Should()
			    .Equal(cycled.GetType(), cycled, cycled.Message, cycled);
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}

		enum SerializationStages
		{
			OnSerializing,
			OnSerialized
		}

		enum DeserializationStages
		{
			OnActivating,
			OnActivated,
			OnDeserialized
		}

		sealed class SerializationMonitor : ISerializationMonitor
		{
			readonly List<(SerializationStages, object)> _stages;

			public SerializationMonitor(List<(SerializationStages, object)> stages) => _stages = stages;

			public void OnSerializing(IFormatWriter writer, object instance)
			{
				_stages.Add((SerializationStages.OnSerializing, instance));
			}

			public void OnSerialized(IFormatWriter writer, object instance)
			{
				_stages.Add((SerializationStages.OnSerialized, instance));
			}

			public void OnActivating(IFormatReader reader, TypeInfo activating) {}

			public void OnActivated(object parameter) {}

			public void OnDeserialized(IFormatReader reader, object instance) {}
		}

		sealed class DeserializationMonitor : ISerializationMonitor
		{
			readonly List<(DeserializationStages, object)> _stages;

			public DeserializationMonitor(List<(DeserializationStages, object)> stages) => _stages = stages;

			public void OnSerializing(IFormatWriter writer, object instance) {}

			public void OnSerialized(IFormatWriter writer, object instance) {}

			public void OnActivating(IFormatReader reader, TypeInfo activating)
			{
				_stages.Add((DeserializationStages.OnActivating, activating));
			}

			public void OnActivated(object parameter)
			{
				_stages.Add((DeserializationStages.OnActivated, parameter));
			}

			public void OnDeserialized(IFormatReader reader, object instance)
			{
				_stages.Add((DeserializationStages.OnDeserialized, instance));
			}
		}

		sealed class Serializer : ISerializer<string>
		{
			readonly ISerializer<string> _serializer;

			public Serializer(ISerializer<string> serializer) => _serializer = serializer;

			public string Get(IFormatReader parameter)
				=> $"- Before Deserialization -{_serializer.Get(parameter)}- After Deserialization -";

			public void Write(IFormatWriter writer, string instance)
			{
				// Before
				_serializer.Write(writer, $"Serialized: {instance}");
				// After
			}
		}
	}
}