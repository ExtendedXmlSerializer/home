using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Registration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.Xml;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue156Tests
	{
		[Fact]
		public void VerifyRegisteredCustomConverterReplacesExistingConverter()
		{
			var serializer = new ConfigurationContainer().Type<DateTime>()
			                                             .Register(DateTimeConverter.Default)
			                                             .Create()
			                                             .ForTesting();

			serializer.Cycle(new Subject {DateTime = DateTime.Now})
			          .DateTime.Should()
			          .Be(new DateTime(1976, 6, 7));
		}

		[Fact]
		public void VerifyUnregisterReturnsTrueThenFalse()
		{
			var container = new ConfigurationContainer().Type<DateTime>();
			container.Register(DateTimeConverter.Default);

			var entry = container.Entry(RegisteredSerializersProperty<DateTime>.Default);
			entry.Get().Should().NotBeNull();

			entry.Remove.Executed();

			entry.Get().Should().BeNull();
		}

		sealed class Subject
		{
			public DateTime DateTime { get; set; }
		}

		sealed class DateTimeConverter : IConverter<DateTime>
		{
			public static DateTimeConverter Default { get; } = new DateTimeConverter();
			DateTimeConverter() : this(new DateTime(1976, 6, 7)) {}

			readonly DateTime _dateTime;
			readonly string _text;

			public DateTimeConverter(DateTime dateTime) : this(dateTime, XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.RoundtripKind)) {}

			public DateTimeConverter(DateTime dateTime, string text)
			{
				_dateTime = dateTime;
				_text = text;
			}

			public DateTime Parse(string data) => _dateTime;

			public string Format(DateTime instance) => _text;
		}

	}
}
