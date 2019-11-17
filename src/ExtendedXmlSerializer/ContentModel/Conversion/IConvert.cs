namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	/// <summary>
	/// Core converter used for converting values of the provided type to and from its text equivalent.
	/// </summary>
	/// <typeparam name="T">The type to convert.</typeparam>
	public interface IConvert<T>
	{
		/// <summary>
		/// Converts the provided text into an instance of the configured convert type.
		/// </summary>
		/// <param name="data">The text string to convert into an instance.</param>
		/// <returns>The instance created from the provided text.</returns>
		T Parse(string data);

		/// <summary>
		/// Converts the provided instance into its text equivalent.
		/// </summary>
		/// <param name="instance">The instance to convert into text.</param>
		/// <returns>The text that represents the provided instance.</returns>
		string Format(T instance);
	}
}