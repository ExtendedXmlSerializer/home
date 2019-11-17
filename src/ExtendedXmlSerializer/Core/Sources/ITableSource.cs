using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// A selector with assignment and a specification.
	/// </summary>
	/// <typeparam name="TKey">The key type.</typeparam>
	/// <typeparam name="TValue">The value type.</typeparam>
	public interface ITableSource<in TKey, TValue>
		: ISpecification<TKey>,
		  IParameterizedSource<TKey, TValue>,
		  IAssignable<TKey, TValue>
	{
		/// <summary>
		/// Removes the value (if any) found with the specified key.
		/// </summary>
		/// <param name="key">The key used to query.</param>
		/// <returns>Result of removal.</returns>
		bool Remove(TKey key);
	}
}