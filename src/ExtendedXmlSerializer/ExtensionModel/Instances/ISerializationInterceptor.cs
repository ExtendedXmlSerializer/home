using ExtendedXmlSerializer.ContentModel.Format;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	/// <inheritdoc />
	public interface ISerializationInterceptor : ISerializationInterceptor<object> {}

	/// <summary>
	/// Provides a mechanism to partake in the serialization pipeline.
	/// </summary>
	/// <typeparam name="T">The type to serialize.</typeparam>
	/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/451" />
	public interface ISerializationInterceptor<T>
	{
		/// <summary>
		/// Called when an instance is being serialized.
		/// </summary>
		/// <param name="writer">The writer performing the serialization.</param>
		/// <param name="instance">The instance to serialize.</param>
		/// <returns>The actual instance to serialize.</returns>
		T Serializing(IFormatWriter writer, T instance);

		/// <summary>
		/// Returns the instance to activate during deserialization.  If `null` is returned, the default internal infrastructure is used to activate the object.
		/// </summary>
		/// <param name="instanceType">The type to activate.</param>
		/// <returns>An instance of the provided type to activate.  If `null`, the default internal activation is used.</returns>
		T Activating(Type instanceType);

		/// <summary>
		/// Called after an object has been activated and properties have been assigned their values from deserialization.
		/// </summary>
		/// <param name="reader">The reader used to deserialize the provided instance</param>
		/// <param name="instance">The instance that was deserialized.</param>
		/// <returns>The actual instance deserialized.</returns>
		T Deserialized(IFormatReader reader, T instance);
	}
}
