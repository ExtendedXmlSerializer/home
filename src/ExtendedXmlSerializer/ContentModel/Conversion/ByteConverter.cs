using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class ByteConverter : Converter<sbyte>
	{
		public static ByteConverter Default { get; } = new ByteConverter();

		ByteConverter() : base(XmlConvert.ToSByte, XmlConvert.ToString) {}
	}
}