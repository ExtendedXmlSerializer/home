using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
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
			var serializer = new ConfigurationContainer().WithDefaultMonitor(new SerializationMonitor(list))
			                                             .Create();
			list.Should()
			    .BeEmpty();
			var instance = new Subject {Message = "Hello World!"};
			var cycled   = serializer.Cycle(instance);
			cycled.Should().BeEquivalentTo(instance);
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
			var serializer = new ConfigurationContainer().WithDefaultMonitor(new DeserializationMonitor(list))
			                                             .Create();
			list.Should()
			    .BeEmpty();
			var instance = new Subject {Message = "Hello World!"};
			var cycled   = serializer.Cycle(instance);
			cycled.Should().BeEquivalentTo(instance);
			list.Select(x => x.Item1)
			    .Should()
			    .Equal(DeserializationStages.OnDeserializing,
			           DeserializationStages.OnActivating, DeserializationStages.OnActivated,
			           DeserializationStages.OnDeserializing, DeserializationStages.OnDeserialized,
			           DeserializationStages.OnDeserialized);
			var objects = list.Select(x => x.Item2)
			                  .ToArray();
			objects.Should()
			       .Equal(cycled.GetType(), cycled.GetType(), cycled, typeof(string), cycled.Message, cycled);
		}

		[Fact]
		void VerifyDefaultWithTypeSpecificRegistration()
		{
			var @default = new List<(DeserializationStages, object)>();
			var specific = new List<(DeserializationStages, string)>();
			var serializer = new ConfigurationContainer().WithDefaultMonitor(new DeserializationMonitor(@default))
			                                             .Type<string>()
			                                             .WithMonitor(new SpecificDeserializationMonitor(specific))
			                                             .Create();
			@default.Should()
			        .BeEmpty();
			specific.Should()
			        .BeEmpty();
			var instance = new Subject {Message = "Hello World!"};
			var cycled   = serializer.Cycle(instance);
			cycled.Should().BeEquivalentTo(instance);
			@default.Select(x => x.Item1)
			        .Should()
			        .Equal(DeserializationStages.OnDeserializing,
			               DeserializationStages.OnActivating, DeserializationStages.OnActivated,
			               DeserializationStages.OnDeserialized);
			@default.Select(x => x.Item2)
			        .Should()
			        .Equal(cycled.GetType(), cycled.GetType(), cycled, cycled);

			specific.Select(x => x.Item1)
			        .Should()
			        .Equal(DeserializationStages.OnDeserialized);
			specific.Select(x => x.Item2)
			        .Should()
			        .Equal(cycled.Message);
		}

		[Fact]
		void VerifyTypeSpecificRegistration()
		{
			var specific = new List<(DeserializationStages, string)>();
			var serializer = new ConfigurationContainer().Type<string>()
			                                             .WithMonitor(new SpecificDeserializationMonitor(specific))
			                                             .Create();
			specific.Should()
			        .BeEmpty();
			var instance = new Subject {Message = "Hello World!"};
			var cycled   = serializer.Cycle(instance);
			cycled.Should().BeEquivalentTo(instance);
			specific.Select(x => x.Item1)
			        .Should()
			        .Equal(DeserializationStages.OnDeserialized);
			specific.Select(x => x.Item2)
			        .Should()
			        .Equal(cycled.Message);
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
			OnDeserializing,
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

			public void OnActivating(IFormatReader reader, Type instanceType) {}

			public void OnActivated(object parameter) {}

			public void OnDeserializing(IFormatReader reader, Type instanceType) {}

			public void OnDeserialized(IFormatReader reader, object instance) {}
		}

		sealed class DeserializationMonitor : ISerializationMonitor
		{
			readonly List<(DeserializationStages, object)> _stages;

			public DeserializationMonitor(List<(DeserializationStages, object)> stages) => _stages = stages;

			public void OnSerializing(IFormatWriter writer, object instance) {}

			public void OnSerialized(IFormatWriter writer, object instance) {}

			public void OnActivating(IFormatReader reader, Type instanceType)
			{
				_stages.Add((DeserializationStages.OnActivating, instanceType));
			}

			public void OnActivated(object parameter)
			{
				_stages.Add((DeserializationStages.OnActivated, parameter));
			}

			public void OnDeserializing(IFormatReader reader, Type instanceType)
			{
				_stages.Add((DeserializationStages.OnDeserializing, instanceType));
			}

			public void OnDeserialized(IFormatReader reader, object instance)
			{
				_stages.Add((DeserializationStages.OnDeserialized, instance));
			}
		}

		sealed class SpecificDeserializationMonitor : ISerializationMonitor<string>
		{
			readonly List<(DeserializationStages, string)> _stages;

			public SpecificDeserializationMonitor(List<(DeserializationStages, string)> stages) => _stages = stages;

			public void OnSerializing(IFormatWriter writer, string instance) {}

			public void OnSerialized(IFormatWriter writer, string instance) {}

			public void OnActivating(IFormatReader reader, Type instanceType) {}

			public void OnActivated(string parameter) {}

			public void OnDeserializing(IFormatReader reader, Type instanceType) {}

			public void OnDeserialized(IFormatReader reader, string instance)
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