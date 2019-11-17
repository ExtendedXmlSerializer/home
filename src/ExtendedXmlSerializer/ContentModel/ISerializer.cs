namespace ExtendedXmlSerializer.ContentModel
{
	/// <summary>
	/// A generalized implementation of <see cref="ISerializer{T}"/>
	/// </summary>
	public interface ISerializer : ISerializer<object>, IReader, IWriter {}

	/// <summary>
	/// A root-level object that combines the <see cref="IReader{T}"/> and <see cref="IWriter{T}"/> for deserialization and serialization, respectively.
	/// </summary>
	/// <typeparam name="T">The subject type to serialize and deserialize.</typeparam>
	public interface ISerializer<T> : IReader<T>, IWriter<T> {}
}