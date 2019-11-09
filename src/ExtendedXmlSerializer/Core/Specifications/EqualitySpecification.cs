namespace ExtendedXmlSerializer.Core.Specifications
{
	sealed class EqualitySpecification<T> : ISpecification<T>
	{
		readonly T _source;

		public EqualitySpecification(T source) => _source = source;

		public bool IsSatisfiedBy(T parameter) => Equals(parameter, _source);
	}
}