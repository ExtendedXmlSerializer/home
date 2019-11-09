using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ExtensionModel.Expressions;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class ValidMarkupExtensionConstructor : IValidConstructorSpecification
	{
		readonly ImmutableArray<IEvaluation> _candidates;

		public ValidMarkupExtensionConstructor(ImmutableArray<IEvaluation> candidates) => _candidates = candidates;

		public bool IsSatisfiedBy(ConstructorInfo parameter)
		{
			var parameters = parameter.GetParameters()
			                          .Select(x => x.ParameterType.GetTypeInfo())
			                          .ToArray();
			var result = parameters.Length == _candidates.Length &&
			             parameters.Zip(_candidates.ToArray(), (info, evaluation) => evaluation.IsSatisfiedBy(info))
			                       .All(x => x);
			return result;
		}
	}
}