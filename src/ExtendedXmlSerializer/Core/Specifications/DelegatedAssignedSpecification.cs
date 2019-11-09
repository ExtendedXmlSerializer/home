using System;

namespace ExtendedXmlSerializer.Core.Specifications
{
	class DelegatedAssignedSpecification<TParameter, TResult> : ISpecification<TParameter>
	{
		readonly static Func<TResult, bool> Specification = AssignedSpecification<TResult>.Default.IsSatisfiedBy;

		readonly Func<TParameter, TResult> _source;
		readonly Func<TResult, bool>       _specification;

		public DelegatedAssignedSpecification(Func<TParameter, TResult> source) : this(source, Specification) {}

		DelegatedAssignedSpecification(Func<TParameter, TResult> source, Func<TResult, bool> specification)
		{
			_source        = source;
			_specification = specification;
		}

		public bool IsSatisfiedBy(TParameter parameter) => _specification(_source(parameter));
	}
}