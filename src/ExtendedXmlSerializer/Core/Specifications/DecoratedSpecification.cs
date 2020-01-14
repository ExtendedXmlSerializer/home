namespace ExtendedXmlSerializer.Core.Specifications
{
	/// <inheritdoc />
	public class DecoratedSpecification<T> : ISpecification<T>
	{
		readonly ISpecification<T> _specification;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specification"></param>
		public DecoratedSpecification(ISpecification<T> specification) => _specification = specification;

		/// <inheritdoc />
		public virtual bool IsSatisfiedBy(T parameter) => _specification.IsSatisfiedBy(parameter);
	}
}