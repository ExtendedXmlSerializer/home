using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class ShortConverter : Converter<short>
	{
		public static ShortConverter Default { get; } = new ShortConverter();

		ShortConverter() : base(XmlConvert.ToInt16, XmlConvert.ToString) {}
	}
}