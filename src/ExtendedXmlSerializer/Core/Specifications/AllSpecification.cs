using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core.Specifications
{
	public class AllSpecification<T> : ISpecification<T>
	{
		readonly ImmutableArray<ISpecification<T>> _specifications;

		public AllSpecification(params ISpecification<T>[] specifications)
			=> _specifications = specifications.ToImmutableArray();

		public bool IsSatisfiedBy(T parameter)
		{
			var length = _specifications.Length;
			for (var i = 0; i < length; i++)
			{
				if (!_specifications[i]
					    .IsSatisfiedBy(parameter))
				{
					return false;
				}
			}

			return true;
		}
	}
}