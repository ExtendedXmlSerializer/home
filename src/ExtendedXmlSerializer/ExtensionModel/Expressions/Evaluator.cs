using System;
using ExtendedXmlSerializer.ExtensionModel.Coercion;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	sealed class Evaluator : IEvaluator
	{
		readonly ICoercers            _coercers;
		readonly IExpressionEvaluator _evaluator;

		public Evaluator(ICoercers coercers, IExpressionEvaluator evaluator)
		{
			_coercers  = coercers;
			_evaluator = evaluator;
		}

		public IEvaluation Get(IExpression parameter)
		{
			var expression = parameter.ToString();
			try
			{
				var instance = parameter is LiteralExpression ? expression : parameter.Get(_evaluator);
				return new Evaluation(_coercers, expression, instance);
			}
			catch (Exception e)
			{
				return new Evaluation(_coercers, expression, e);
			}
		}
	}
}