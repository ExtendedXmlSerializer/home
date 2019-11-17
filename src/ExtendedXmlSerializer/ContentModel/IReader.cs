using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel
{
	/// <summary>
	/// A generalized implementation of <see cref="IReader{T}"/>
	/// </summary>
	public interface IReader : IReader<object> {}

	/// <summary>
	/// Used during deserialization to materialize an object of the given type by the provided format reader.
	/// </summary>
	/// <typeparam name="T">The type to materialize.</typeparam>
	public interface IReader<out T> : IParameterizedSource<IFormatReader, T> {}
}