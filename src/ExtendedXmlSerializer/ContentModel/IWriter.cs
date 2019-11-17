using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	/// <summary>
	/// A generalized implementation of <see cref="IWriter{T}"/>.
	/// </summary>
	public interface IWriter : IWriter<object> {}

	/// <summary>
	/// Used during serialization to emit the provided instance (and its contents, if necessary) to the provided writer.
	/// </summary>
	/// <typeparam name="T">The subject to emit.</typeparam>
	public interface IWriter<in T>
	{
		/// <summary>
		/// Emits the instance into the provided writer.
		/// </summary>
		/// <param name="writer">The writer that represents the destination.</param>
		/// <param name="instance">The instance to emit.</param>
		void Write(IFormatWriter writer, T instance);
	}
}