using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class UnsignedLongConverter : Converter<ulong>
	{
		public static UnsignedLongConverter Default { get; } = new UnsignedLongConverter();

		UnsignedLongConverter() : base(XmlConvert.ToUInt64, XmlConvert.ToString) {}
	}
}