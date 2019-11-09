using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ReflectionModel;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue180Tests
	{
		[Fact]
		void Verify()
		{
			var container = new ConfigurationContainer()
			                .Extend(Extension.Default)
			                .Create()
			                .ForTesting();
			var result = container.Cycle(new Subject {Message = "Creating"});
			result.Message.Should()
			      .Be("Hello World! Creating Hello Again!");
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}

		sealed class Extension : ISerializerExtension
		{
			public static Extension Default { get; } = new Extension();

			Extension() {}

			public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.DecorateContent<Content>(IsAssignableSpecification<string>.Default);

			public void Execute(IServices parameter) {}
		}

		sealed class Content : IContents
		{
			public ISerializer Get(TypeInfo parameter) => Serializer.Default.Adapt();
		}

		sealed class Serializer : ISerializer<string>
		{
			public static Serializer Default { get; } = new Serializer();

			Serializer() {}

			public string Get(IFormatReader parameter) => $"{parameter.Content()} Hello Again!";

			public void Write(IFormatWriter writer, string instance)
			{
				writer.Content($"Hello World! {instance}");
			}
		}
	}
}