using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sprache;
using ExtendedXmlSerializer.ExtensionModel.Markup;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	sealed class Expression : Parsing<IExpression>
	{
		readonly static Parser<IExpression> Text = new TextLiteral('\'').ToParser()
		                                                                .XOr(new TextLiteral('"'));

		public Expression(params char[] except) : base(Text.XOr(new ContainedExpression(except))) {}
	}
}