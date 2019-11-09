using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sprache;
using ExtendedXmlSerializer.ExtensionModel.Expressions;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class ContainedExpression : Parsing<IExpression>
	{
		public ContainedExpression(params char[] delimiters) : base(Parse.CharExcept(delimiters)
		                                                                 .AtLeastOnce()
		                                                                 .Text()
		                                                                 .Token()
		                                                                 .Select(x => new GeneralExpression(x))) {}
	}
}