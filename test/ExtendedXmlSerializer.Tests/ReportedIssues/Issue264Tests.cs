using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue264Tests
	{
		[Fact]
		void Verify()
		{
			new ConfigurationContainer().Type<string>()
			                            .RegisterContentComposition(x => new Subject(x))
			                            .Create()
			                            .Cycle("Hello World!")
			                            .Should()
			                            .Be("- Before Deserialization -Serialized: Hello World!- After Deserialization -");
		}

		sealed class Subject : ISerializer<string>
		{
			readonly ISerializer<string> _serializer;

			public Subject(ISerializer<string> serializer) => _serializer = serializer;

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