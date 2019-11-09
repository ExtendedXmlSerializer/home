namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	sealed class ExpressionsExtension : ISerializerExtension
	{
		public static ExpressionsExtension Default { get; } = new ExpressionsExtension();

		ExpressionsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<IExpressionEvaluator>(ExpressionEvaluator.Default)
			            .Register<IEvaluator, Evaluator>();

		public void Execute(IServices parameter) {}
	}
}