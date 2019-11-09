namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	sealed class GeneralExpression : ExpressionBase
	{
		public GeneralExpression(string expression) : base(expression) {}

		public override object Get(IExpressionEvaluator parameter) => parameter.Get(ToString());
	}
}