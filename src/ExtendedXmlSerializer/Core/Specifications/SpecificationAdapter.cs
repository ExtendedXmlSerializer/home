namespace ExtendedXmlSerializer.Core.Specifications
{
	sealed class SpecificationAdapter<T> : ISpecification<object>
	{
		readonly ISpecification<T> _specification;

		public SpecificationAdapter(ISpecification<T> specification) => _specification = specification;

		public bool IsSatisfiedBy(object parameter) => parameter is T && _specification.IsSatisfiedBy((T)parameter);
	}
}