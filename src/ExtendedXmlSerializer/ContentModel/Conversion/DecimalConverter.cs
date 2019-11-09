using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class DecimalConverter : Converter<decimal>
	{
		public static DecimalConverter Default { get; } = new DecimalConverter();

		DecimalConverter() : base(XmlConvert.ToDecimal, XmlConvert.ToString) {}
	}
}