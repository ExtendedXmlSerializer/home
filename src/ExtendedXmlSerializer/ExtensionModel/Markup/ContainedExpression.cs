using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.ExtensionModel.Expressions;
using Sprache;

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