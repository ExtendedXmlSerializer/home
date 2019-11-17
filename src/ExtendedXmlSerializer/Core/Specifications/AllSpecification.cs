using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core.Specifications
{
	/// <inheritdoc />
	public class AllSpecification<T> : ISpecification<T>
	{
		readonly ImmutableArray<ISpecification<T>> _specifications;

		/// <inheritdoc />
		public AllSpecification(params ISpecification<T>[] specifications)
			=> _specifications = specifications.ToImmutableArray();

		/// <inheritdoc />
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