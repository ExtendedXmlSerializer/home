using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class AlteredSpecification<T> : DecoratedSpecification<T>
	{
		readonly IAlteration<T> _alteration;

		public AlteredSpecification(IAlteration<T> alteration, ISpecification<T> specification) : base(specification)
			=> _alteration = alteration;

		public override bool IsSatisfiedBy(T parameter) => base.IsSatisfiedBy(_alteration.Get(parameter));
	}
}