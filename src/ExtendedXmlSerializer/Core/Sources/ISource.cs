namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// A general purpose interface that contains (or resolves) a value of an instance of the given type.
	/// </summary>
	/// <typeparam name="T">The instance type.</typeparam>
	public interface ISource<out T>
	{
		/// <summary>
		/// Retrieves the value within this container object.
		/// </summary>
		/// <returns>The resulting instance.</returns>
		T Get();
	}
}