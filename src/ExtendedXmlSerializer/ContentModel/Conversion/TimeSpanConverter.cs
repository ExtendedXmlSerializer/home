using System;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class TimeSpanConverter : Converter<TimeSpan>
	{
		public static TimeSpanConverter Default { get; } = new TimeSpanConverter();

		TimeSpanConverter() : base(XmlConvert.ToTimeSpan, XmlConvert.ToString) {}
	}
}