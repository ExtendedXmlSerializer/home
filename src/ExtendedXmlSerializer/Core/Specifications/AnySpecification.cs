using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core.Specifications
{
	class AnySpecification<T> : ISpecification<T>
	{
		readonly ImmutableArray<ISpecification<T>> _specifications;

		public AnySpecification(params ISpecification<T>[] specifications) =>
			_specifications = specifications.ToImmutableArray();

		public bool IsSatisfiedBy(T parameter)
		{
			var length = _specifications.Length;
			for (var i = 0; i < length; i++)
			{
				if (_specifications[i]
					.IsSatisfiedBy(parameter))
				{
					return true;
				}
			}

			return false;
		}
	}
}