using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class FloatConverter : Converter<float>
	{
		public static FloatConverter Default { get; } = new FloatConverter();

		FloatConverter() : base(XmlConvert.ToSingle, XmlConvert.ToString) {}
	}
}