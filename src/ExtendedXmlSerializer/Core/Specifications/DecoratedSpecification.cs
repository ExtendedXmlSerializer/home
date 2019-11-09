namespace ExtendedXmlSerializer.Core.Specifications
{
	public class DecoratedSpecification<T> : ISpecification<T>
	{
		readonly ISpecification<T> _specification;

		public DecoratedSpecification(ISpecification<T> specification) => _specification = specification;

		public virtual bool IsSatisfiedBy(T parameter) => _specification.IsSatisfiedBy(parameter);
	}
}