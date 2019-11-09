using System.Collections.Generic;
using ExtendedXmlSerializer.Core.NReco.LambdaParser.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	sealed class ExpressionEvaluator : IExpressionEvaluator
	{
		public static ExpressionEvaluator Default { get; } = new ExpressionEvaluator();

		ExpressionEvaluator() {}

		public object Get(string parameter) => new LambdaParser().Eval(parameter, new Dictionary<string, object>());
	}
}