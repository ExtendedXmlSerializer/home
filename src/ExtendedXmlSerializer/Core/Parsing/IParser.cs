using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core.Parsing
{
	/// <summary>
	/// Selector that creates an instance of the specified type based on the provided text string.
	/// </summary>
	/// <typeparam name="T">The type to create.</typeparam>
	public interface IParser<out T> : IParameterizedSource<string, T> {}
}