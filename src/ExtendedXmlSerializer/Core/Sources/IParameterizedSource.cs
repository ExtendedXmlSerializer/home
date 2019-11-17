namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// A general purpose selection component that accepts a value and returns a value.
	/// </summary>
	/// <typeparam name="TParameter">The type to accept.</typeparam>
	/// <typeparam name="TResult">The return type.</typeparam>
	public interface IParameterizedSource<in TParameter, out TResult>
	{
		/// <summary>
		/// Performs the selection.
		/// </summary>
		/// <param name="parameter">The parameter to accept.</param>
		/// <returns>A value of the return type.</returns>
		TResult Get(TParameter parameter);
	}
}