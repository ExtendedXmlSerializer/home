using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class UnsignedByteConverter : Converter<byte>
	{
		public static UnsignedByteConverter Default { get; } = new UnsignedByteConverter();

		UnsignedByteConverter() : base(XmlConvert.ToByte, XmlConvert.ToString) {}
	}
}