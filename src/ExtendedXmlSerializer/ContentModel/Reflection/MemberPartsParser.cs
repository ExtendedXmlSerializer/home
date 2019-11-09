using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class MemberPartsParser : Parsing<MemberParts>
	{
		public static MemberPartsParser Default { get; } = new MemberPartsParser();

		MemberPartsParser() : this(TypePartsParser.Default, DefaultClrDelimiters.Default.Separator,
		                           CodeIdentifier.Default) {}

		public MemberPartsParser(Parser<TypeParts> type, Parser<char> separator, Parser<string> identifier) : base(
		                                                                                                           type
			                                                                                                           .SelectMany(separator
			                                                                                                                       .Then(identifier
				                                                                                                                             .Accept)
			                                                                                                                       .Accept,
			                                                                                                                       (parts,
			                                                                                                                        name)
				                                                                                                                       => new
					                                                                                                                       MemberParts(parts,
					                                                                                                                                   name))
		                                                                                                          ) {}
	}
}