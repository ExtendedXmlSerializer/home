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
		void VerifyAwareness()
		{
			var list = new List<(Stages, object)>();
			var serializer = new ConfigurationContainer().EnableAwareness(new Awareness(list))
			                                             .Create();
			list.Should()
			    .BeEmpty();
			var instance = new Subject{ Message = "Hello World!"};
			var cycled = serializer.Cycle(instance);
			cycled.ShouldBeEquivalentTo(instance);
			list.Select(x => x.Item1)
			    .Should()
			    .Equal(Stages.OnActivating, Stages.OnActivated, Stages.OnDeserialized);
			list.Last()
			    .Item2.Should()
			    .BeSameAs(cycled);
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}

		enum Stages
		{
			OnActivating,
			OnActivated,
			OnDeserialized
		}

		sealed class Awareness : IDeserializationAware
		{
			readonly List<(Stages, object)> _stages;

			public Awareness(List<(Stages, object)> stages) => _stages = stages;

			public void OnActivating(TypeInfo activating, IFormatReader reader)
			{
				_stages.Add((Stages.OnActivating, activating));
			}

			public void OnActivated(object parameter)
			{
				_stages.Add((Stages.OnActivated, parameter));
			}

			public void OnDeserialized(object instance, IFormatReader reader)
			{
				_stages.Add((Stages.OnDeserialized, instance));
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