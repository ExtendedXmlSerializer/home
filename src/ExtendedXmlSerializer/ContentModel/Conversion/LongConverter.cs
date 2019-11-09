using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class LongConverter : Converter<long>
	{
		public static LongConverter Default { get; } = new LongConverter();

		LongConverter() : base(XmlConvert.ToInt64, XmlConvert.ToString) {}
	}
}