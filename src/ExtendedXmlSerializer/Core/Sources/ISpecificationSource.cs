using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.Core.Sources
{
	public interface ISpecificationSource<in TParameter, out TResult>
		: ISpecification<TParameter>, IParameterizedSource<TParameter, TResult> {}
}