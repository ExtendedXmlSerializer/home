using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class UnsignedIntegerConverter : Converter<uint>
	{
		public static UnsignedIntegerConverter Default { get; } = new UnsignedIntegerConverter();

		UnsignedIntegerConverter() : base(XmlConvert.ToUInt32, XmlConvert.ToString) {}
	}
}