using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <inheritdoc />
	public class SpecificationSource<TParameter, TResult> : ISpecificationSource<TParameter, TResult>
	{
		readonly ISpecification<TParameter>                _specification;
		readonly IParameterizedSource<TParameter, TResult> _source;

		/// <inheritdoc />
		public SpecificationSource(IParameterizedSource<TParameter, TResult> source) :
			this(source.IfAssigned(), source) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specification"></param>
		/// <param name="source"></param>
		public SpecificationSource(ISpecification<TParameter> specification,
		                           IParameterizedSource<TParameter, TResult> source)
		{
			_specification = specification;
			_source        = source;
		}

		/// <inheritdoc />
		public bool IsSatisfiedBy(TParameter parameter) => _specification.IsSatisfiedBy(parameter);

		/// <inheritdoc />
		public TResult Get(TParameter parameter) => _source.Get(parameter);
	}
}