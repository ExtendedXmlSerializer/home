using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	interface IExpression : IParameterizedSource<IExpressionEvaluator, object> {}
}