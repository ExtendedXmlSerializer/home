using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue156Tests
	{
		[Fact]
		public void VerifyRegisteredCustomConverterReplacesExistingConverter()
		{
			var serializer = new ConfigurationContainer().Type<DateTime>()
			                                             .Register()
			                                             .Converter()
			                                             .Using(DateTimeConverter.Default)
			                                             .Create()
			                                             .ForTesting();

			serializer.Cycle(new Subject {DateTime = DateTime.Now})
			          .DateTime.Should()
			          .Be(new DateTime(1976, 6, 7));
		}

		[Fact]
		public void VerifyNoneRemovesConverter()
		{
			var container = new ConfigurationContainer();
			var extension = container.Root.With<ConvertersExtension>();

			extension.Converters.Count(x => x.IsSatisfiedBy(typeof(DateTime).GetTypeInfo()))
			         .Should()
			         .BeGreaterOrEqualTo(1);

			container.Type<DateTime>().Register().Converter().None();
			extension.Converters.Count(x => x.IsSatisfiedBy(typeof(DateTime).GetTypeInfo()))
			         .Should()
			         .Be(0);
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
			readonly string   _text;

			public DateTimeConverter(DateTime dateTime)
				: this(dateTime,
				       ExtendedXmlSerializer.ContentModel.Conversion.DateTimeConverter.Default.Format(dateTime)) {}

			public DateTimeConverter(DateTime dateTime, string text)
			{
				_dateTime = dateTime;
				_text     = text;
			}

			public bool IsSatisfiedBy(TypeInfo parameter) => parameter.AsType() == typeof(DateTime);

			public DateTime Parse(string data) => _dateTime;

			public string Format(DateTime instance) => _text;
		}
	}
}