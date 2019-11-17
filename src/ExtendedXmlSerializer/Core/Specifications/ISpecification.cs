namespace ExtendedXmlSerializer.Core.Specifications
{
	/// <summary>
	/// Resolves a condition based on the provided parameter.
	/// </summary>
	/// <typeparam name="T">The parameter type</typeparam>
	public interface ISpecification<in T>
	{
		/// <summary>
		/// Returns a boolean based on the provided parameter.
		/// </summary>
		/// <param name="parameter">The parameter from which to resolve the condition.</param>
		/// <returns>The condition based on the provided parameter.</returns>
		bool IsSatisfiedBy(T parameter);
	}
}