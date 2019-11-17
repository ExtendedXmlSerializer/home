using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// A selector paired with a specification.
	/// </summary>
	/// <typeparam name="TParameter">The selector's parameter type.</typeparam>
	/// <typeparam name="TResult">The selection result type.</typeparam>
	public interface ISpecificationSource<in TParameter, out TResult>
		: ISpecification<TParameter>, IParameterizedSource<TParameter, TResult> {}
}