using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	sealed class Coercers : ICoercers
	{
		readonly IEnumerable<ICoercer> _coercers;

		public Coercers(IEnumerable<ICoercer> coercers) => _coercers = coercers;

		public ICoercion Get(object parameter)
		{
			var candidates = Yield(parameter)
				.ToImmutableArray();
			var result = candidates.Any() ? new Context(parameter, candidates) : null;
			return result;
		}

		IEnumerable<ICoercer> Yield(object parameter)
		{
			foreach (var candidate in _coercers)
			{
				if (candidate.IsSatisfiedBy(parameter))
				{
					yield return candidate;
				}
			}
		}

		sealed class Context : AnySpecification<TypeInfo>, ICoercion
		{
			readonly object                   _instance;
			readonly ImmutableArray<ICoercer> _candidates;

			public Context(object instance, ImmutableArray<ICoercer> candidates)
				: base(candidates.ToArray<ISpecification<TypeInfo>>())
			{
				_instance   = instance;
				_candidates = candidates;
			}

			public object Get(TypeInfo parameter)
			{
				foreach (var candidate in _candidates)
				{
					if (candidate.IsSatisfiedBy(parameter))
					{
						return candidate.Get(new CoercerParameter(_instance, parameter));
					}
				}

				return null;
			}
		}
	}
}