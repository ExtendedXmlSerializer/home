namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	sealed class LiteralExpression : ExpressionBase
	{
		public LiteralExpression(string text) : base(text) {}

		public override object Get(IExpressionEvaluator parameter) => ToString();
	}
}