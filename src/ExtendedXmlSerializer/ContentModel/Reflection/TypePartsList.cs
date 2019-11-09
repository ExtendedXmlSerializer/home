using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypePartsList : ItemsParser<TypeParts>
	{
		public static TypePartsList Default { get; } = new TypePartsList();

		TypePartsList() : base(Parse.Ref(() => TypePartsParser.Default.ToParser())) {}
	}
}