using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using Sprache;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class EscapedLiteral : Parsing<string>
	{
		public EscapedLiteral(char containingCharacter)
			: base(Parse.String("{}")
			            .Then(Parse.CharExcept(containingCharacter)
			                       .Many()
			                       .Text()
			                       .Accept)) {}
	}
}