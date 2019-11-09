using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class UnsignedShortConverter : Converter<ushort>
	{
		public static UnsignedShortConverter Default { get; } = new UnsignedShortConverter();

		UnsignedShortConverter() : base(XmlConvert.ToUInt16, XmlConvert.ToString) {}
	}
}