namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// A specialized selection that accepts and returns an instance of the same type.
	/// </summary>
	/// <typeparam name="T">The type instance to alter.</typeparam>
	public interface IAlteration<T> : IParameterizedSource<T, T> {}
}