using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Xunit;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue154Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().CustomSerializer<string, BasicCustomSerializer>()
			                                             .Create()
			                                             .ForTesting();

			var data =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?><string xmlns=\"https://extendedxmlserializer.github.io/system\">Custom String!</string>";
			serializer.Assert("Subject String", data);

			serializer.Deserialize<string>(data)
			          .Should()
			          .Be("Hello world!");
		}

		[Fact]
		public void VerifySingleton()
		{
			var serializer = new ConfigurationContainer().CustomSerializer<string, BasicCustomSerializerSingleton>()
			                                             .Create()
			                                             .ForTesting();

			var data =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?><string xmlns=\"https://extendedxmlserializer.github.io/system\">Custom String!</string>";
			serializer.Assert("Subject String", data);

			serializer.Deserialize<string>(data)
			          .Should()
			          .Be("Hello world!");
		}

		[Fact]
		public void VerifyActivation()
		{
			var serializer = new ConfigurationContainer().CustomSerializer<Subject, SerializerWithDependencies>()
			                                             .CustomSerializer<string, BasicCustomSerializer>()
			                                             .Create()
			                                             .ForTesting();

			var data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue154Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"">Custom String!</Issue154Tests-Subject>";
			serializer.Assert(new Subject(), data);

			serializer.Deserialize<Subject>(data)
			          .Message.Should()
			          .Be("Hello world!");
		}

		[Fact]
		public void VerifySerializer()
		{
			var serializer = new ConfigurationContainer().Type<string>()
			                                             .Register()
			                                             .Serializer()
			                                             .Using(BasicSerializer.Default)
			                                             .Create()
			                                             .ForTesting();

			var serialize = serializer.Serialize("Hello???");
			serialize.Should()
			         .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><string xmlns=""https://extendedxmlserializer.github.io/system"">Hello World!</string>");

			serializer.Deserialize<string>(serialize)
			          .Should()
			          .Be("Hello World!!");
		}

		[Fact]
		public void VerifySerializerActivated()
		{
			var serializer = new ConfigurationContainer().Type<string>()
			                                             .Register()
			                                             .Serializer()
			                                             .Of<BasicSerializer>()
			                                             .Create()
			                                             .ForTesting();

			var serialize = serializer.Serialize("Hello???");
			serialize.Should()
			         .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><string xmlns=""https://extendedxmlserializer.github.io/system"">Hello World!</string>");

			serializer.Deserialize<string>(serialize)
			          .Should()
			          .Be("Hello World!!");
		}

		[Fact]
		public void VerifySerializerActivatedDependency()
		{
			var serializer = new ConfigurationContainer().Type<string>()
			                                             .Register()
			                                             .Serializer()
			                                             .Of(typeof(ActivatedSerializer))
			                                             .Create()
			                                             .ForTesting();

			var serialize = serializer.Serialize("Hello???");
			serialize.Should()
			         .Be(@"<?xml version=""1.0"" encoding=""utf-8""?><string xmlns=""https://extendedxmlserializer.github.io/system"">The registered encoding is: utf-8</string>");

			serializer.Deserialize<string>(serialize)
			          .Should()
			          .Be("Hello World from ActivatedSerializer");
		}

		sealed class BasicSerializer : ISerializer<string>
		{
			public static BasicSerializer Default { get; } = new BasicSerializer();

			BasicSerializer() {}

			public string Get(IFormatReader parameter) => "Hello World!!";

			public void Write(IFormatWriter writer, string instance)
			{
				writer.Content("Hello World!");
			}
		}

		sealed class ActivatedSerializer : ISerializer<string>
		{
			readonly Encoding _encoding;

			public ActivatedSerializer(Encoding encoding) => _encoding = encoding;

			public string Get(IFormatReader parameter) => "Hello World from ActivatedSerializer";

			public void Write(IFormatWriter writer, string instance)
			{
				writer.Content($"The registered encoding is: {_encoding.WebName}");
			}
		}

		sealed class BasicCustomSerializer : IExtendedXmlCustomSerializer<string>
		{
			public string Deserialize(XElement xElement) => "Hello world!";

			public void Serializer(XmlWriter xmlWriter, string obj)
			{
				xmlWriter.WriteString("Custom String!");
			}
		}

		sealed class BasicCustomSerializerSingleton : IExtendedXmlCustomSerializer<string>
		{
			[UsedImplicitly]
			public static BasicCustomSerializerSingleton Default { get; } = new BasicCustomSerializerSingleton();

			BasicCustomSerializerSingleton() {}

			public string Deserialize(XElement xElement) => "Hello world!";

			public void Serializer(XmlWriter xmlWriter, string obj)
			{
				xmlWriter.WriteString("Custom String!");
			}
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}

		sealed class SerializerWithDependencies : IExtendedXmlCustomSerializer<Subject>
		{
			readonly IParameterizedSource<TypeInfo, IExtendedXmlCustomSerializer> _serializers;

			public SerializerWithDependencies(ICustomXmlSerializers serializers) => _serializers = serializers;

			public Subject Deserialize(XElement xElement) => new Subject
			{
				Message = (string)_serializers.Get(typeof(string))
				                              .Deserialize(xElement)
			};

			public void Serializer(XmlWriter xmlWriter, Subject obj)
			{
				_serializers.Get(typeof(string))
				            .Serializer(xmlWriter, obj.Message);
			}
		}
	}
}