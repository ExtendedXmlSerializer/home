using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class DoubleConverter : Converter<double>
	{
		public static DoubleConverter Default { get; } = new DoubleConverter();

		DoubleConverter() : base(XmlConvert.ToDouble, XmlConvert.ToString) {}
	}
}