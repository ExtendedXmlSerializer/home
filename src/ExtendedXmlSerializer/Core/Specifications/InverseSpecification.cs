namespace ExtendedXmlSerializer.Core.Specifications
{
	class InverseSpecification<T> : DelegatedSpecification<T>
	{
		public InverseSpecification(ISpecification<T> inner) : base(inner.IsSatisfiedBy) {}

		public override bool IsSatisfiedBy(T parameter) => !base.IsSatisfiedBy(parameter);
	}
}