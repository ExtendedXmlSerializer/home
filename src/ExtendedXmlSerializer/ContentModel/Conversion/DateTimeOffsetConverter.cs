using System;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class DateTimeOffsetConverter : Converter<DateTimeOffset>
	{
		public static DateTimeOffsetConverter Default { get; } = new DateTimeOffsetConverter();

		DateTimeOffsetConverter() : base(XmlConvert.ToDateTimeOffset, XmlConvert.ToString) {}
	}
}