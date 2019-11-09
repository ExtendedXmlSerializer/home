namespace ExtendedXmlSerializer.Core.Specifications
{
	sealed class NullAwareSpecificationAdapter<T> : ISpecification<object>
	{
		readonly ISpecification<T> _specification;

		public NullAwareSpecificationAdapter(ISpecification<T> specification) => _specification = specification;

		public bool IsSatisfiedBy(object parameter) => parameter == null
			                                               ? _specification.IsSatisfiedBy(default)
			                                               : parameter is T &&
			                                                 _specification.IsSatisfiedBy((T)parameter);
	}
}