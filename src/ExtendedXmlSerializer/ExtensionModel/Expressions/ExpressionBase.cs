namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	abstract class ExpressionBase : IExpression
	{
		readonly string _expression;

		protected ExpressionBase(string expression)
		{
			_expression = expression;
		}

		public abstract object Get(IExpressionEvaluator parameter);

		public override string ToString() => _expression;
	}
}