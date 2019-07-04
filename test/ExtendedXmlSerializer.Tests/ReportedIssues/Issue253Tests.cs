using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Reflection;
using System.Text;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue253Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new ClassWithEncodingProperty{ Encoding = Encoding.ASCII };

			var serializer = new ConfigurationContainer().Register<Encoding, EncodingSerializer>()
			                                             .Create()
			                                             .ForTesting();
			serializer.Cycle(instance).ShouldBeEquivalentTo(instance);
		}

		public class ClassWithEncodingProperty
		{
			public ClassWithEncodingProperty() => Encoding = Encoding.Default;

			public Encoding Encoding { get; set; }
		}

		sealed class EncodingSerializer : ISerializer<Encoding>
		{
			readonly ISerializer _serializer;

			public EncodingSerializer(IContents contents) : this(contents.Get(typeof(int).GetTypeInfo())) {}

			public EncodingSerializer(ISerializer serializer) => _serializer = serializer;

			public Encoding Get(IFormatReader parameter) => Encoding.GetEncoding((int)_serializer.Get(parameter));

			public void Write(IFormatWriter writer, Encoding instance)
			{
				_serializer.Write(writer, instance.CodePage);
			}
		}

	}
}
