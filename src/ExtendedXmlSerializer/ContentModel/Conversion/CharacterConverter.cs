using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class CharacterConverter : Converter<char>
	{
		public static CharacterConverter Default { get; } = new CharacterConverter();

		CharacterConverter() : base(XmlConvert.ToChar, XmlConvert.ToString) {}
	}
}