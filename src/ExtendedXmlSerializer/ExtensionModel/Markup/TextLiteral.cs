using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sprache;
using ExtendedXmlSerializer.ExtensionModel.Expressions;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class TextLiteral : Parsing<IExpression>
	{
		const char Slash = '\\';

		readonly static Parser<char> EscapedCharacter = Parse.Char(Slash)
		                                                     .Then(Parse.CharExcept(Slash)
		                                                                .Accept);

		public TextLiteral(CharacterParser containingCharacter) : this(containingCharacter, containingCharacter) {}

		public TextLiteral(CharacterParser containingCharacter, Parser<char> container) : base(
		                                                                                       new
				                                                                                       EscapedLiteral(containingCharacter
					                                                                                                      .Character)
			                                                                                       .ToParser()
			                                                                                       .XOr(
			                                                                                            Parse
				                                                                                            .CharExcept(
				                                                                                                        $"{containingCharacter.Character}{Slash}")
				                                                                                            .Or(EscapedCharacter)
				                                                                                            .Many()
				                                                                                            .Text()
			                                                                                           )
			                                                                                       .Contained(container,
			                                                                                                  container)
			                                                                                       .Token()
			                                                                                       .Select(x => new
				                                                                                               LiteralExpression(x))
		                                                                                      ) {}
	}
}