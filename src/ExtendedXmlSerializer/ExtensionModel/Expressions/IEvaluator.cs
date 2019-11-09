using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	interface IEvaluator : IParameterizedSource<IExpression, IEvaluation> {}
}