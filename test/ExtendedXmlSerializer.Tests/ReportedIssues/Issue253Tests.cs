using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
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
			var serializer = new ConfigurationContainer().Type<Encoding>()
			                                             .Register()
			                                             .Serializer()
			                                             .Of<EncodingSerializer>()
			                                             .Create()
			                                             .ForTesting();

			var instance                  = new ClassWithEncodingProperty {Encoding = Encoding.ASCII};
			var classWithEncodingProperty = serializer.Cycle(instance);
			classWithEncodingProperty.Should()
			                         .BeEquivalentTo(instance);
		}

		[Fact]
		void VerifyAlternativeRegister()
		{
			var instance = new ClassWithEncodingProperty {Encoding = Encoding.ASCII};

			var serializer = new ConfigurationContainer().Type<Encoding>()
			                                             .Register()
			                                             .Serializer()
			                                             .Of(typeof(EncodingSerializer))
			                                             .Create()
			                                             .ForTesting();
			serializer.Cycle(instance)
			          .Should()
			          .BeEquivalentTo(instance);
		}

		[Fact]
		void VerifyAdvanced()
		{
			var instance = new Settings {DbSettings = new DBVCSV {Encoding = Encoding.ASCII}};
			var serializer = new ConfigurationContainer().Type<Encoding>()
			                                             .Register()
			                                             .Serializer()
			                                             .Of<EncodingSerializer>()
			                                             .Create()
			                                             .ForTesting();
			serializer.Cycle(instance)
			          .Should()
			          .BeEquivalentTo(instance);
		}

		public class Settings
		{
			public DbSettings DbSettings { [UsedImplicitly] get; set; }
		}

		public class DbSettings
		{
			public Encoding Encoding { [UsedImplicitly] get; set; }
		}

		public class DBVCSV : DbSettings {}

		public class ClassWithEncodingProperty
		{
			public ClassWithEncodingProperty() => Encoding = Encoding.Default;

			public Encoding Encoding { [UsedImplicitly] get; set; }
		}

		sealed class EncodingSerializer : ISerializer<Encoding>
		{
			readonly ISerializer _serializer;

			[UsedImplicitly]
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