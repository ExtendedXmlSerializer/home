using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Tests.Support;
using System;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue242Tests
	{
		[Fact]
		void Verify()
		{
			var time     = new DateTime(2019, 3, 5);
			var instance = new Subject {Value = time};

			new ConfigurationContainer().Type<Subject>()
			                            .Member(subject => subject.Value)
			                            .Register(Serializer.Default)
			                            .Create()
			                            .ForTesting()
			                            .Assert(instance,
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue242Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Value>2019-03-05</Value></Issue242Tests-Subject>");
		}

		sealed class Serializer : ISerializer<DateTime>
		{
			public static Serializer Default { get; } = new Serializer();

			Serializer() : this(DateConverter.Default) {}

			readonly IConverter<DateTime> _converter;

			public Serializer(IConverter<DateTime> converter) => _converter = converter;

			public DateTime Get(IFormatReader parameter) => _converter.Parse(parameter.Content());

			public void Write(IFormatWriter writer, DateTime instance)
			{
				writer.Content(_converter.Format(instance));
			}
		}

		sealed class DateConverter : Converter<DateTime>
		{
			public static DateConverter Default { get; } = new DateConverter();

			DateConverter() : this(DateTimeConverter.Default) {}

			public DateConverter(IConvert<DateTime> convert)
				: base(convert.Parse, time => time.ToString("yyyy-MM-dd")) {}
		}

		public sealed class Subject
		{
			[XmlAttribute(DataType = "ID")]
			public string id { get; set; }

			[XmlText(DataType = "date")]
			public DateTime Value { get; set; }
		}
	}
}