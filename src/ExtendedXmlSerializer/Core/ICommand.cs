namespace ExtendedXmlSerializer.Core
{
	/// <summary>
	/// A parameterized command that performs operations against the provided parameter.
	/// </summary>
	/// <typeparam name="T">The parameter type.</typeparam>
	public interface ICommand<in T>
	{
		/// <summary>
		/// Executes an operation against the provided parameter.
		/// </summary>
		/// <param name="parameter">The input parameter.</param>
		void Execute(T parameter);
	}
}