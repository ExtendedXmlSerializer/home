using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class IntegerConverter : Converter<int>
	{
		public static IntegerConverter Default { get; } = new IntegerConverter();

		IntegerConverter() : base(XmlConvert.ToInt32, XmlConvert.ToString) {}
	}
}