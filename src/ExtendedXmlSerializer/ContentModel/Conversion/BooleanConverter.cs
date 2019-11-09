using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class BooleanConverter : Converter<bool>
	{
		public static BooleanConverter Default { get; } = new BooleanConverter();

		BooleanConverter() : base(XmlConvert.ToBoolean, XmlConvert.ToString) {}
	}
}