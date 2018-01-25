using System;
using System.Reflection;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue156Tests
	{
		[Fact]
		public void VerifyRegisteredCustomConverterReplacesExistingConverter()
		{
			var serializer = new ConfigurationContainer().Register(DateTimeConverter.Default)
			                                             .Create()
			                                             .ForTesting();

			serializer.Cycle(new Subject {DateTime = DateTime.Now})
			          .DateTime.Should()
			          .Be(new DateTime(1976, 6, 7));
		}

		[Fact]
		public void VerifyUnregisterReturnsTrueThenFalse()
		{
			var container = new ConfigurationContainer();
			container.Unregister(DateTimeConverter.Default).Should().BeTrue();
			container.Unregister(DateTimeConverter.Default).Should().BeFalse();
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

			public DateTimeConverter(DateTime dateTime) : this(dateTime, ExtendedXmlSerializer.ContentModel.Conversion.DateTimeConverter.Default.Format(dateTime)) {}

			public DateTimeConverter(DateTime dateTime, string text)
			{
				_dateTime = dateTime;
				_text = text;
			}

			public bool IsSatisfiedBy(TypeInfo parameter) => parameter.AsType() == typeof(DateTime);

			public DateTime Parse(string data) => _dateTime;

			public string Format(DateTime instance) => _text;
		}

	}
}
