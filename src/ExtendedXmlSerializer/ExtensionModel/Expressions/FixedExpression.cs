using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	class FixedExpression<T> : IExpression, ISource<T>
	{
		readonly T _instance;

		public FixedExpression(T instance)
		{
			_instance = instance;
		}

		public object Get(IExpressionEvaluator parameter) => Get();

		public T Get() => _instance;
	}
}