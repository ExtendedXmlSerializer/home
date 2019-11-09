using System;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	public sealed class DateTimeConverter : ConverterBase<DateTime>
	{
		public static DateTimeConverter Local { get; } = new DateTimeConverter(XmlDateTimeSerializationMode.Local);

		public static DateTimeConverter Default { get; } = new DateTimeConverter();

		DateTimeConverter() : this(XmlDateTimeSerializationMode.RoundtripKind) {}

		readonly XmlDateTimeSerializationMode _mode;

		public DateTimeConverter(XmlDateTimeSerializationMode mode) => _mode = mode;

		public override DateTime Parse(string data) => XmlConvert.ToDateTime(data, _mode);

		public override string Format(DateTime instance) => XmlConvert.ToString(instance, _mode);
	}
}