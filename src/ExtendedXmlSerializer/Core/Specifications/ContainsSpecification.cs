using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Specifications
{
	class ContainsSpecification<T> : DelegatedSpecification<T>
	{
		public ContainsSpecification(ICollection<T> source) : base(source.Contains) {}

		public sealed override bool IsSatisfiedBy(T parameter) => base.IsSatisfiedBy(parameter);
	}
}