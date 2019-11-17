namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// A selector that derives a text string representation from the provided instance.
	/// </summary>
	/// <typeparam name="T">The instance type.</typeparam>
	public interface IFormatter<in T> : IParameterizedSource<T, string> {}
}